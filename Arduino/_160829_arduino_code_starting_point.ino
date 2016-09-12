/*
Drive ShelfLab 2pt0 with Arduino

 */

// Setup for LEDs

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

// Setup for Screen

int thresh[15]= {1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000,1000,1000,1000};
const int planogram[15]={6,16,26,36,46,56,66,76,90,100,110,120,130,140,150};

long weight[40]= {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
byte recal=false;
long recal_t=millis()+5000;

#define poll 8
#define clock 9
#define illum 10
#define sensorpin A0
#define threshold -1
#define nshelves 1
#define nfacings 7

// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

  int timer = 1000;           // The higher the number, the slower the timing.
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
  
  	FastLED.addLeds<NEOPIXEL,DATA_PIN>(leds, NUM_LEDS);
  
  // Setup alarm
  pinMode(13, OUTPUT); 
  
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

for (int pass =0; pass <3; pass++)

{

active = false;
strcpy(packet, "");

  // set poll high for first sensor only
   digitalWrite(poll, HIGH);   
   
  int i = 0;
  
  for (int shelf = 1; shelf<nshelves+1; shelf++)
  {
    for (int pos = 1; pos <(nfacings+1); pos++)
    {

    // advance clock by one
    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
    digitalWrite(clock, LOW); 
    
    // switch off poll
    digitalWrite(poll, LOW);       
    
    // advance clock by one
//    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
//    digitalWrite(clock, LOW); 

    // illuminate
    digitalWrite(illum, HIGH); 
    delayMicroseconds(timer);    
    //  read AIN unillum goes here
    sensorill = analogRead(sensorpin);    
 
     // unilluminate
    digitalWrite(illum, LOW);
    delayMicroseconds(timer);  
    //  read AIN illum goes here
    sensorunill = analogRead(sensorpin);    
    
    delay(1);
    
    diff = max(0,sensorill-sensorunill);
    
    diff=1024-diff;

    tempstr ="";
 /*tempstr += i + 1;
    tempstr += ",";
    tempstr += sensorill;
    tempstr += ",";
    tempstr += sensorunill;
    tempstr += ",";
    tempstr += thresh[i];
    tempstr += ",";
  */  
    
    if (diff > thresh[i]) {  
    weight[i] += diff;
    weight[i] -= thresh[i];
    // tempstr += "*,";
        
    // packet = packet + (i + 1) + "," + diff + ",";
    // Serial.print(i + 1);
    // Serial.print(",");
    // Serial.print(diff);
    // Serial.print(",");
    active = true;
  } 

    //tempstr.toCharArray(temp,20);
    //strcat(packet, temp);

    if (recal==true) thresh[i]=diff+90;    
    // math goes here
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

if (millis()>time_off)
{
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
	// Set the i'th led to red 
	leds[l] = CRGB::White;
        }
FastLED.show();
}

void unilluminate (int facing)
{
  	for(int l = start_led[facing]; l < end_led[facing]; l++) 
        {
	// Set the i'th led to red 
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

