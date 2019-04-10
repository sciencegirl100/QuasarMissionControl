/* USB Serial NeoPixel Control System (Name Tentative)
 *  I have chosen to use the FastLED Library available:
 *    Here: https://github.com/FastLED/FastLED
 *    Or in the Library Manager of the Arduino IDE
 *  This library sacrifices some program and memory 
 *  space for lower-latency.
 *  
 *  Serial Interface:
 *  "rgbse" Red byte, Green byte, Blue byte, Start byte, end byte
*/

#include <FastLED.h>
#include "settings.h"

CRGB strip[LED_COUNT];
String inputString = "";
bool stringComplete = false;
char command[6];

void setup() {
  // This is setup to use the cheap WS2812 strips.
  // See the FastLED docs to adapt to your strip
  FastLED.addLeds<WS2812, PIN, BRG>(strip, LED_COUNT);
  Serial.begin(BAUDRATE);
  inputString.reserve(200);
}

void loop() {
  if (stringComplete) {
    // ensure message isn't too small for a command
    // EX: 255,255,255,0,136 or 0,0,0,0,1
    if (inputString.length() > 9){
      int red = getValue(inputString, 0).toInt();
      int green = getValue(inputString, 1).toInt();
      int blue = getValue(inputString, 2).toInt();
      int startPoint = getValue(inputString, 3).toInt();
      int endPoint = getValue(inputString, 4).toInt();

      for (int i = startPoint; i < endPoint+1; i++){
         strip[i].setRGB(red, green, blue);
      }

      Serial.println(String(red) + ":" + String(green) + ":" + String(blue) + " S:" + String(startPoint) + " E:" + String(endPoint)/*inputString*/);
      
      FastLED.show();
    }
    inputString = "";
    stringComplete = false;
  }
}

String getValue(String data, int index) {
  // This is from :
  // https://arduino.stackexchange.com/a/1237
  int found = 0;
  int strIndex[] = { 0, -1 };
  int maxIndex = data.length() - 1;
  char separator = ',';
  for (int i = 0; i <= maxIndex && found <= index; i++) {
    if (data.charAt(i) == separator || i == maxIndex) {
      found++;
      strIndex[0] = strIndex[1] + 1;
      strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }
  return found > index ? data.substring(strIndex[0], strIndex[1]) : "";
}

void serialEvent() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    inputString += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}
