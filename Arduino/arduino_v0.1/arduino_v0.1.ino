
#include "FastLED.h"

//pin number of data pin
#define DATA_PIN 5
//pin number for the array of sensors. all sensors in the array
//use the same pin
#define SENSORS 8
//pin number for the clock
#define CLOCK 9
//pin number to communicate with the IR emitter for the current sensor
#define SENSOR_EMITTER 10
//pin number to communicate with the IR receiver for the current sensor
#define SENSOR_RECEIVER A0
//number of shelfRokr units
#define NUM_SHELVES 1
//number of facings on a single shelfRokr
#define NUM_SLOTS 7
//delay time to allow the emitter to warm up to full strength. time is in MS
#define EMITTER_TIMER 1000
//initial value for thresholds. a low starting value will not effect the flow
//of logic and will balance out after the first few iterations
#define THRESHOLD_DEFAULT 512
//percent different a sensor value has to be to be considered picked up
#define PICKUP_RATIO .5
//delay time in ms between sensor poll loops
#define LOOP_DELAY 500

//boolean values to maintain picked up state of slots. false
//indicates that the product is not picked up
bool pickedUp[NUM_SLOTS];
//to store threshold values for each sensor
int thresholds[NUM_SLOTS];

//number of LEDS on the strip
#define NUM_LEDS 25
//reference to the LED array for FASTLed
CRGB leds[NUM_LEDS];
//start led index for its corresponding product slot
const int LEDStart[] = {0, 3, 6, 9, 12, 16, 21};
//ending led index for its corresponding product slot
const int LEDEnd[] = {2, 5, 8, 12, 15, 19, 24};
//bool values to keep track of which slots have blinking LEDs
bool slotsBlinking[NUM_SLOTS];
//keeps track of the brightness value for updateLighting()
int brightness = 0;
//increment value for brightness to fade in and out
int brightnessIncrement = 15;
//global that keeps track if a chasing effect is playing
bool chasingEffect = false;
//the lead LED in the chase effect must be between 0 and NUM_LEDS
//should always start at 0
int chaseLeader = 0;
//the direction the chase is traveling. should always start as true
//or will interfere with chaseEffect logic
bool chaseRight = true;


// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

void setup() {
  //initialize boolean values to to start as false because all
  //slots should have something in it
  //Serial.print("initializing thresholds");
  initializeThresholds();
  initializeSlots();
  LEDSetup();
  
  //informs FastLED library the reference for the LED array and how many there are
  /*FastLED.addLeds<NEOPIXEL, DATA_PIN>(leds, NUM_LEDS);*/

  //sets up special function registers for the arduino. not sure of particulars,
  //but this is standard for all arduino programs.
  // set prescale to 16
  sbi(ADCSRA, ADPS2) ;
  cbi(ADCSRA, ADPS1) ;
  cbi(ADCSRA, ADPS0) ;

  //set up communication modes for pins
  pinMode(SENSORS, OUTPUT);
  pinMode(CLOCK, OUTPUT);
  pinMode(SENSOR_EMITTER, OUTPUT);
  Serial.begin(9600);
  
}

//arduino main loop
void loop() {
  //check the sensors to see if anything has been picked up or put down
  pollProductSensors();
  
  //receive any commands from the main program
  readInput();

  //update the lighting effects
  updateLighting();
}

//reads through all of the product sensors to see which have been picked up
void pollProductSensors(){
  
  //send signal to activate the first sensor
  digitalWrite(SENSORS, HIGH);

  //iterate through each shelf and sensor
  for(int i = 0; i < NUM_SHELVES; i++){
    for(int j = 0; j < NUM_SLOTS; j++){
      
      //advances clock and iterates current sensor to the
      //next sensor in the array
      digitalWrite(CLOCK, HIGH);
      digitalWrite(CLOCK, LOW);
      //switch off current sensor so the previous sensor is the only one active
      digitalWrite(SENSORS, LOW);
      
      //actually check to see if the status of each slot has changed
      sensorWork(j);
      
    }

    //delay to allow time between polling all of the sensors. accounts for slower pickups
    delay(LOOP_DELAY);
    
  }
}

//checks if the product is being picked up
boolean sensorWork(int slot){
  //values of SENSOR_RECEIVER when emitter is on and off
  int sensorOn = 0;
  //int sensorOff = 0;

  //activate emitter and take a reading then deactivate it and
  //take another reading
  digitalWrite(SENSOR_EMITTER, HIGH);
  delayMicroseconds(EMITTER_TIMER);
  sensorOn = analogRead(SENSOR_RECEIVER);
  digitalWrite(SENSOR_EMITTER, LOW);
  delayMicroseconds(EMITTER_TIMER);
  //sensorOff = analogRead(SENSOR_RECEIVER);

  if(pickedUp[slot] == false && sensorOn < thresholds[slot] * PICKUP_RATIO){
    sendPickUp(slot);
  }else if(pickedUp[slot] == true && sensorOn > thresholds[slot] * PICKUP_RATIO){
    sendPutDown(slot);
  }

  //only want to update the threshold if the product has been sitting in place
  if(pickedUp[slot] == false && slot == 0){
    updateThresholds(sensorOn, slot);
  }
}

//updates the threshold value to it reflects a more accurate mid-range value
void updateThresholds(int sensorOn, int slot){
  thresholds[slot] = (thresholds[slot] + sensorOn) / 2;
}

//sends message to host machine to indidcate that there was a pickup
void sendPickUp(int slot){
  pickedUp[slot] = true;
  Serial.print(slot);
  Serial.print('U');
  Serial.print('\n');
}

//sends message to host machine to indicate that there was a putdown
void sendPutDown(int slot){
  pickedUp[slot] = false;
  Serial.print(slot);
  Serial.print('D');
  Serial.print('\n');
}

//reads input from the host machine and parses it to detemine how to call the
//triggerLighting function
void readInput(){
  if(Serial.available() > 0){
    String command = Serial.readStringUntil('\n');
    int slot = -1;

    //using if block with function calls to allow for
    //extensibility in the future for other commandsS
    if(command.startsWith("LED")){
      parseCommandLED(command);
    }else{
      //not a valid command. can be altered later for additional commands
    }
  }
}

void parseCommandLED(String command){
  //char ledCommand[] = command;
  //command.toCharArray(ledCommand, command.length());
  
  char action = command.charAt(4);
  int slot = (int)(command.charAt(6)) - 48;
  if(action == 'U'){
    stopChaseEffect();
    activateSlot(slot);
  }else if(action == 'D'){
    stopChaseEffect();
    deactivateSlot(slot);
  }else if(action == 'C'){
    beginChaseEffect(0);
  }else{
    //do nothing. clear buffer. bad command
  }
}

void updateLighting(){
  if(chasingEffect){    //code for simulating the chase effect
    if(chaseRight){
      leds[chaseLeader - 2] = CRGB::Black;
      if(chaseLeader >= NUM_LEDS - 1){
        chaseRight == false;
      }else{
        chaseLeader++;
      }
    }else if(!chaseRight){
      leds[chaseLeader + 2] = CRGB::Black;
      if(chaseLeader <= 0){
        chaseRight = true;
      }else{
        chaseLeader--;
      }
    }else{
      //should not get here. it should be either one of the above
    }

    leds[chaseLeader - 1] = CRGB::White;
    leds[chaseLeader] = CRGB::White;
    leds[chaseLeader + 1] = CRGB::White;

    FastLED.show();
  
  }else{    //iterate through all the slots and do light effect for items that are picked up
    brightness += brightnessIncrement;
    FastLED.setBrightness(brightness);
    FastLED.show();
    
    if((brightness >= 255 && brightnessIncrement > 0) || (brightness <= 0 && brightnessIncrement < 0)){
      brightnessIncrement *= -1;
    }
  }
}

void activateSlot(int slot){
  for(int i = LEDStart[slot]; i < LEDEnd[slot]; i++){
    leds[i] = CRGB::White;
  }
}

void deactivateSlot(int slot){
  for(int i = LEDStart[slot]; i < LEDEnd[slot]; i++){
    leds[i] = CRGB::Black;
  }
}

//starts the chasing effect at the designated slot
void beginChaseEffect(int slot){
  chaseLeader = LEDStart[slot];
  leds[chaseLeader] = CRGB::White;
  FastLED.setBrightness(255);
  chasingEffect = true;
}

//cancels the chasing effect
void stopChaseEffect(){
  chasingEffect = false;
  //reset the chase
  FastLED.setBrightness(0);
  chaseLeader = 0;
  
  //code to reset the LEDs back to black
  for(int i = LEDStart[chaseLeader - 1]; i <= LEDEnd[chaseLeader + 1]; i++){
    leds[i] = CRGB::Black;
  }
  FastLED.show();
  
}

//called in the beginning of the program. iterates over the pickedUp array
//and sets all values to false, which is the default value. a value of 
//false indicates that the product is still sitting in the slot.
void initializeSlots(){
  for(int i = 0; i < NUM_SLOTS; i++){
    pickedUp[i] = false;
  }
}

//called at the beginning of the program. sets all of the threshold values to
//a default of 512, which is the middle range of analog values that can be sent
//by the sensor
void initializeThresholds(){
  for(int i = 0; i < NUM_SLOTS; i++){
    thresholds[i] = THRESHOLD_DEFAULT;
  }
}

//sets up the LED array to be used by the FastLED library
void LEDSetup(){
  FastLED.addLeds<NEOPIXEL,DATA_PIN>(leds, NUM_LEDS);
  for(int i = 0; i < NUM_SLOTS; i++){
    slotsBlinking[i] = false;
  }
}

