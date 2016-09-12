/*
Drive ShelfLab 2pt0 with Arduino

 */

// Setup for Screen

#include <G4D.h>
#include <SoftwareSerial.h>

#define POWER_PIN      11
#define RESET_PIN      12
#define SOFT_TX        3
#define SOFT_RX        2

// color mapping
const byte r[32] = {0, 0, 0, 14, 42, 71, 99, 128, 128, 128, 128, 128, 128,128,128,128};
const byte g[32] = {0, 0, 0, 14, 42, 71, 99, 128,128,128,128, 0, 0, 0, 0, 0};
const byte b[32] = {128, 128, 128, 114, 85, 55, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0};


SoftwareSerial oledSerial(SOFT_RX, SOFT_TX);
G4D display(POWER_PIN,RESET_PIN,&oledSerial);

#define poll 8
#define clock 9
#define illum 10
#define sensorpin A0
#define threshold 2

// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

  int timer = 200000;           // The higher the number, the slower the timing.
  int n=9;
  long count=0;
  int sensorill=0;
  int sensorunill=0;
  int diff = 0;
  byte active = false;
  char packet[100] ="";
  char temp[20]="";
  String tempstr="";
  long start;
  
  int buzzer = 11;    // buzzer pin
  int length = 75;     // for tone length
  int lowfreq = 1200; // the lowest frequency value to use
  int highfreq = 1800; //  the highest...
  int limit = 5;   // the limit
  int countreps;     // counter
  int swit;        // switch

void setup() {
  
  // Setup alarm
  pinMode(buzzer, OUTPUT);
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
  
oledSerial.begin(57600);
display.powerDown();
display.powerUp();
//stringG(uint8_t x, uint8_t y, uint8_t font, uint8_t r, uint8_t g, uint8_t b,  uint8_t width,  uint8_t height, char str[] );
display.stringG(9, 20, 0, 128, 128, 128,  2,  2, "ShelfL" );
display.stringG(98, 20, 0, 128, 128, 128,  2,  2, "B" );
display.stringG(110, 30, 0, 0, 128, 0,  1,  1, "2.0" );
// pen(uint8_t size);
display.pen(1);
// triangle(uint8_t x1, uint8_t y1, uint8_t x2, uint8_t y2, uint8_t x3, uint8_t y3, uint8_t r, uint8_t g, uint8_t b);
display.triangle(95, 33, 88, 20, 81, 33, 0, 0, 255);
display.pen(0);

digitalWrite(sensorpin,LOW);

}

void loop() {

  // set poll high for first sensor only
   digitalWrite(poll, HIGH);   

    // print time stamp
    Serial.print(millis());
    Serial.print(",");  
    strcpy(packet, "");
  int i = 0;
  
  for (int shelf = 1; shelf<2; shelf++)
  {
    for (int pos = 1; pos <7; pos++)
    {
      forward(2);
      
     // illuminate
//    digitalWrite(illum, HIGH);
    //delayMicroseconds(timer);  
    delay(20);
    //  read AIN illum goes here
    sensorill = analogRead(sensorpin);    
    
    // unilluminate
//    digitalWrite(illum, LOW); 
//    delayMicroseconds(timer);    
    //  read AIN unillum goes here
    sensorunill = analogRead(sensorpin);    
    
//    diff = max(0,sensorill-sensorunill);
    diff = max(0,sensorunill);
    
    if (shelf ==1)
    {
    barwrite(shelf, pos, diff);
    }
    else 
    {
      barwrite(shelf, 7-pos,diff);
    }
    
//    if (diff > threshold) {
    tempstr ="";
    tempstr += i + 1;
    tempstr += ",";
    tempstr += diff;
    tempstr += ",";
    tempstr.toCharArray(temp,11);
    strcat(packet, temp);    
        
    // packet = packet + (i + 1) + "," + diff + ",";
    // Serial.print(i + 1);
    // Serial.print(",");
    // Serial.print(diff);
    // Serial.print(",");
    active = true;
  //} 
    
      // switch off poll
    digitalWrite(poll, LOW);       
    
    // math goes here
     count++;  
     i++;

  }
}
   if (active) {
     start = min(start, millis());
     if (millis()-start>5000) {
     //  alarm();
       start = millis(); 
     }
   }
   else {start = millis();}
   strcat(packet,"x1");
   Serial.println(packet);
   // }
   active = false;
   // display.clear();

}

void alarm()
{
  countreps = 0;

  while (countreps<limit)
  {
    countreps++;
    swit = 1 - swit;
    digitalWrite(13, swit);
    // increasing tone
    for (int m = lowfreq; m<=highfreq; m++)
    {
      tone (buzzer, m, length);
    }
    // decreasing tone
    for (int m = highfreq; m>=lowfreq; m--)
    {
      tone (buzzer, m, length);
    }
  }
  noTone(buzzer);
}

void barwrite(int shelf, int facing, int voltage)
  {  
    int shlfht = (3-shelf)*32+36;
    int val = (voltage >> 6);
    int ht = shlfht-(val);
    int pos = facing*20-15;
    display.rectangle(pos,shlfht-31,pos+18,ht,0,0,0);
    display.rectangle(pos,ht,pos+18,shlfht,r[val],g[val],b[val]);
   }

void forward (int steps)
      {
        for (int k =1; k<steps+1; k++)
        {
    // advance clock by one
    digitalWrite(clock, HIGH); 
    //  delayMicroseconds(timer);                  
    digitalWrite(clock, LOW); 
          // switch off poll
    digitalWrite(poll, LOW); 
      }  
      }
