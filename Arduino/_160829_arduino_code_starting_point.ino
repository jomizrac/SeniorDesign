/*
Drive ShelfLab 2pt0 with Arduino

 */
///////////////////*********** LED Stuff *************///////////////////
//external library that is used for LED animations
#include "FastLED.h"

// How many leds in your strip?
#define NUM_LEDS 25

const int start_led[7] = {0, 3, 6, 9, 13, 16, 21};
const int end_led[7]= {2, 6, 9, 12, 15, 19, 24};

// For led chips like Neopixels, which have a data line, ground, and power, you just
// need to define DATA_PIN.  For led chipsets that are SPI based (four wires - data, clock,
// ground, and power), like the LPD8806, define both DATA_PIN and CLOCK_PIN
#define DATA_PIN 5
#define CLOCK_PIN 13

// Define the array of leds
CRGB leds[NUM_LEDS];

/////////////////////////******************************/////////////////////

// Setup for Screen

//initial values to check the sensorpin against. 15 values is from an old version of the product
int thresh[15]= {1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000,1000,1000,1000};
//not sure if this is still needed. patrick didnt speak much on it
const int planogram[15]={6,16,26,36,46,56,66,76,90,100,110,120,130,140,150};

//used in conjunction with threshold array and recalibrate functionality
long weight[40]= {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
//used to determine if recalibration is needed
byte recal=false;
long recal_t=millis()+5000;

//pin 8 reading part of the sensor. must be made active before it can be read from. this is the entire sensor being made active
#define poll 8	
//pin 9 cycle for which poll operates from. must manuall increment the clock. each incremement moves to the next sensor
#define clock 9	
//pin 10 emitting part of the sensor. emits the signal. must be turned on in order to read from it
#define illum 10
//analog pin A0. reading part of the sensor.used to detect what illum is emitting on 
#define sensorpin A0
//unused variable
#define threshold -1
//number of shelves
#define nshelves 1
//number of facings on a shelf
#define nfacings 7	

//boilerplate stuff that must be used with arduino
// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

///////////******* variables used with playing videos and coordinating LED effects ************/////////////
  int timer = 1000;           // The higher the number, the slower the timing. being used to pause the process
  int n=4;
  long count=0;
  int sensorill=0;
  int sensorunill=0;
  int diff = 0;
  byte active = false;
  byte thanking = false;
  byte present = false;
  byte arrived = false;
  long time_arrived = 0;
  long time_off = millis()+600000;
  long time_trigger = millis()+10000;
  byte looking = false;
  byte left = false;
  byte brightness=255;
  int brightness_inc=-10;
  int facing=0;
  int show_next=0;
  char play_vid='a';
  ////////////////////////////////*****************************************/////////////////////////////////
  
  char packet[300] ="";
  char temp[40]="";
  String tempstr="";
  long start;
  
  int buzzer = 11;    // buzzer pin
  int length = 75;     // for tone length
  int lowfreq = 1200; // the lowest frequency value to use
  int highfreq = 1800; //  the highest...
  int limit = 5;   // the limit
  int countreps;     // counter
  int swit;        // switch
  int slot = 0;

void setup() {
  
  // Setup LEDs
  	//rsgisters to the FastLED library that there is a string of type NEOPIXEL on pin DATA_PIN
  	//and that this object is referenced by the leds variable and that there are NUM_LEDS
  	FastLED.addLeds<NEOPIXEL,DATA_PIN>(leds, NUM_LEDS);
  
  // Setup alarm
  pinMode(13, OUTPUT);
  
  //boilerplate stuff that needs to be included
  // set prescale to 16
  sbi(ADCSRA,ADPS2) ;
  cbi(ADCSRA,ADPS1) ;
  cbi(ADCSRA,ADPS0) ;

  // set pins for poll, clock, illum:
  pinMode(poll, OUTPUT);
  pinMode(clock, OUTPUT);
  pinMode(illum, OUTPUT);
  Serial.begin(9600); 
  
}

void loop() {



    /* print time stamp
    Serial.print(millis());
    Serial.print(",");  
    Serial.print(recal_t);
    Serial.print(","); 
    */

//these for loops are used to iterate over all product facings
for (int pass =0; pass <3; pass++){
//variable used to determine if the current facing has a product in it
active = false;
strcpy(packet, "");

  //sets active current sensor. current sensor is determiend by the clock
  //i think the default start position is the first sensor
   digitalWrite(poll, HIGH);   
   
  int i = 0;
  
  for (int shelf = 1; shelf<nshelves+1; shelf++){	//iterates over number of shelves
    for (int pos = 1; pos <(nfacings+1); pos++){	//??iterates over number of positions on a shelf??

    //used to increment through sensors in conjunction with poll
    //each increment moves to the next sensor in the system
    // advance clock by one
    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
    digitalWrite(clock, LOW); 
    
    //turn off poll so that only one sensor is on at a time
    // switch off poll
    digitalWrite(poll, LOW);       
    
    // advance clock by one
//    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
//    digitalWrite(clock, LOW); 

//////////////////******* sets up value to check for on the sensor *********////////////////
    //activate the LED of the current sensor and wait for it to light up product
    digitalWrite(illum, HIGH); 
    delayMicroseconds(timer);    
    //read the value from the sensor while illuminated and store for reference
    sensorill = analogRead(sensorpin);    
 
     //turn off the led on the sensor and wait for light to disipate from product
    digitalWrite(illum, LOW);
    delayMicroseconds(timer)	//delay by time in micro 
    //read value from sensor while there is no light in order to account for any value due to background noise
    sensorunill = analogRead(sensorpin);    
    
    delay(1);	//delay by time in millis
    
    //subtracts the value of the sensor when its off from the value of when its on. this is an attempt
    //to compensate for background noise. 
    diff = max(0,sensorill-sensorunill);
    
    diff=1024-diff;
//////////////////******* sets up value to check for on the sensor *********////////////////

    //not sure what this is doing
    tempstr ="";
    
    //detecting if what the sensor is picking up is greater than the threshold
    if (diff > thresh[i]) {  
    weight[i] += diff;
    weight[i] -= thresh[i];
    active = true;
  } 

    //tempstr.toCharArray(temp,20);
    //strcat(packet, temp);

    //recallibration routine for the threshold values as they change over time
    if (recal==true) thresh[i]=diff+90;    
    //math goes here
     count++;  
     i++;

  }
}
detect_person();
}

brightness+=brightness_inc;
if (brightness<40)  brightness_inc=+30;
if (brightness>240) brightness_inc=-15;
FastLED.setBrightness(brightness);
FastLED.show();

//i think this plays a background vid
if (millis()>time_off){
  for (int i=0; i<7; i++) unilluminate(i);
  time_off=millis()+300000;
  play_background(play_vid);
  play_vid++;
  if (play_vid=='i') play_vid='a';
}

//   Serial.print(packet);
//   Serial.println(",x1");

   if (active) {
     thanking = true;
     start = min(start, millis());
     if (millis()-start>5) {
     //  alarm();
       active = false;
       start = millis(); 
       recal = true;
     }
   }
   else {
    start = millis();
    if (recal==true)
    {
      recal=false;
      recal_t=millis()+60000;
    }
    else
    {
      if (millis()>recal_t) 
      {
      recal=true;
      //Serial.print("Recalibrating :");
      //Serial.print(recal);
      //Serial.print(", ");
      //Serial.println(millis());
      }
    }
 
      if (thanking == true)
      {
        for (int i=0; i<7; i++)
        {
        unilluminate(i);
        }
        thanking=false;
       long sum_prod = 0;
       long sum_raw = 0;
       for (int i=0; i<15; i++) 
         {
           sum_prod+= weight[i]*(i+1)*10;
           sum_raw+= weight[i];
           weight[i]=0;
         }
       int posn = sum_prod/sum_raw; 
       //Serial.print("Position=");
       //Serial.print(posn);

        slot = 0;
              for (int i=0; i<7; i++) if (posn >= planogram[i]) slot++;

       //Serial.print("Slot=");
       //Serial.println(slot);

    //Serial.println("Before genie write");

    // display  product
    illuminate (slot-1);
    
    play_facing(slot);
    //delay(1000);
    time_off=millis()+45000;
        
    //Serial.println("After genie write");
     recal=true;
           
      }
 
 }
     
}
 
void illuminate (int facing)
{
  	for(int l = start_led[facing]; l < end_led[facing]; l++) 
        {
	// Set the i'th led to white 
	leds[l] = CRGB::White;
        }
FastLED.show();
}

void unilluminate (int facing)
{
  	for(int l = start_led[facing]; l < end_led[facing]; l++) 
        {
	// Set the i'th led to black (turn off)
	leds[l] = CRGB::Black;
        }
FastLED.show();
}

void play_facing(int i)
{
  Serial.print(i);
}

void play_background(char a)
{
  Serial.print(a);
}



void stop_facing(int i)
{
  // pause video goes here
}


void detect_person()
{

int new_present = false;
arrived = false;

tempstr= "";

for (int i=0; i<3; i++)
    {
    // advance clock by one
    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
    digitalWrite(clock, LOW); 
    
   
    // advance clock by one
    digitalWrite(clock, HIGH); 
    delayMicroseconds(timer);                  
    digitalWrite(clock, LOW); 
 
    int range = analogRead(sensorpin);    

    if (range > 200) new_present = true;

    tempstr+= "R";
    tempstr+= i+1;
    tempstr+= ",";
    tempstr+= range;
    tempstr+= ",";
      
   }

    tempstr.toCharArray(temp,20);
    strcat(packet, temp);

if (!new_present && millis()> time_trigger && millis()< time_arrived+600000) flash_facing();

if (new_present && !present) 
      {
        arrived = true;
        present = true;
        time_arrived = millis();
        tempstr+= " Arrived";
      }

    if (present && millis()>time_arrived+200 && !looking  &&!active) 
      {
        looking = true;
        // started_looking = true;
        tempstr+= "Looking";
        play_facing(0);
      }
      
    if (!new_present && present) 
    {
      if (looking)
      {
      play_facing(8);
      tempstr+= "Left";
      active = false;
      delay(6000);
      play_background(play_vid);
      }
      left = true;
      present = false;
      looking = false;
      for (int i=0; i<7; i++) unilluminate(i);

    }
   
}

void flash_facing()
{
play_facing(9);
delay(1250);

for (int count=0; count<7; count++)
{

FastLED.setBrightness( 0 );
illuminate(count);

  FastLED.setBrightness(250);
  FastLED.show();

delay(150);
  FastLED.setBrightness(0);
  FastLED.show();

unilluminate(count);

delay (150);
}

illuminate(show_next);
time_trigger=millis()+300000;
play_facing(show_next+1);
time_off = millis()+90000;
show_next++;
if (show_next==7) show_next=0;

}

