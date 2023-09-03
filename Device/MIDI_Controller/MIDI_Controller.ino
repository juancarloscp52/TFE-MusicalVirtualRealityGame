// Juan Carlos Cebrián Peñuela.

#include <Arduino.h>
#include <BLEMidi.h>
byte size = 16;

byte buttons [16] = {23,22,21,19,18,5,4,15,32,33,25,26,27,14,12,13}; 
byte currentState [16] = {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1};
byte lastState [16] = {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1};

byte previous_start_note = 0;


void setup() {
  Serial.begin(115200);
  Serial.println("Initializing bluetooth");
  BLEMidiServer.begin("Basic MIDI device");
  Serial.println("Waiting for connections...");
  pinMode(2, OUTPUT);

  for (byte i = 0; i<size; i++){
      pinMode(buttons[i], INPUT_PULLUP);
  }
}

int sectorSum = 0;
byte counter = 0;
byte averagedSector = 0;

int start_note = 0;

bool starting_pots=true;
int starting_counter = 0;
int pitch_avg=0;
int pitch_sum=0;
int vol_avg=0;
int vol_sum=0;
int bpm_avg=0;
int bpm_sum=0;

void handle_potentiometer(){

  // Compute potentiometer off-set during start up.
  if(starting_pots){
    pitch_sum+=analogRead(34);
    vol_sum+=analogRead(36);
    bpm_sum+=analogRead(39);
    starting_counter++;
    if(starting_counter % 300 == 0){
      pitch_avg = (pitch_sum/300)-15;
      vol_avg = vol_sum/300;
      bpm_avg = bpm_sum/300;

      starting_pots=false;
    }
    return;
  }

  counter++;

  // Send potentiometer values.
  if(counter % 25 ==0){
    Serial.printf("(%d, %d) Pitch Potentiometer: %d (%d), Volume Potentiometer: %d (%d), Note Interval: %d, bpm potentiomenter: %d (%d) \n", pitch_avg, vol_avg, map(analogRead(34),pitch_avg,4096,0,16383), analogRead(34), map(analogRead(36),vol_avg,4096,0,127), analogRead(36), analogRead(35), map(analogRead(39),bpm_avg,4096,0,127), analogRead(39));
    BLEMidiServer.pitchBend(0,(uint16_t)constrain(map(analogRead(34),pitch_avg,4096,0,16384), 0, 16384));
    BLEMidiServer.controlChange(0,1,(uint8_t)constrain(map(analogRead(36),vol_avg,4096,0,127), 0, 127));
    BLEMidiServer.controlChange(0,0,(uint8_t)constrain(map(analogRead(35),0,4096,127,0), 0, 127));
    BLEMidiServer.controlChange(0,2,(uint8_t)constrain(map(analogRead(39),bpm_avg,4096,0,127), 0, 127));

  }

  // Current note interval mapping. The posible 127 MIDI notes are divided into 7 sector containing 16 notes each.

  int sector = map(analogRead(35),0,4095,7,0);
  sectorSum +=sector;

  // Average note interval potentiometer.
  if(counter % 200 == 0){
    averagedSector = sectorSum/200;
    Serial.printf("Sector raw: %d averaged %d sum %d, start note %d \n", sector, averagedSector, sectorSum, size*averagedSector);
    sectorSum=0;
    counter = 0;
  }

  // Compute starting note.
  start_note = size*averagedSector;
}

void loop() {

  if(BLEMidiServer.isConnected()) { // Wait for connection.
    
    digitalWrite(2, HIGH);  // turn the LED on

    //Handle potenciometer
    handle_potentiometer();

    for(byte i = 0; i < size ; i++){
      // Store pressed buttons
      currentState[i]=digitalRead(buttons[i]);

      // If button state has changed, store last state and send the MIDI note Message.
      if(lastState[i]!=currentState[i]){

        lastState[i]=currentState[i];

        if(!currentState[i]){
          BLEMidiServer.noteOn(0, start_note+i, 70);
        }else{
          BLEMidiServer.noteOff(0, start_note+i, 70);
        }
      }

      // Turn off previous notes if sector has changed.
      if(start_note!=previous_start_note){

        Serial.println("RESETTING");
        BLEMidiServer.noteOff(0, previous_start_note+i, 70);
      }
    }

    previous_start_note=start_note;
    delay(5);

   }else{
    // Blink when no device connected.
    digitalWrite(2, HIGH);
    delay(200);   
    digitalWrite(2, LOW);
    delay(200); 

  }
}