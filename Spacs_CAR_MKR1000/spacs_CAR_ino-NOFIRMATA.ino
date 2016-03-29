#include <WiFi101.h>
#include <WiFiClient.h>
#include <WiFiServer.h>
#include <WiFiSSLClient.h>
#include <WiFiUdp.h>

unsigned int pingDistanceNow;
unsigned int inReleuPort1 = 6; //Brake pin  

////////////////////////////////////////////////     SETUP ELEMENTS     ////////////////////////////////////
void setup() {
  Serial.begin(9600); // Open serial monitor at 9600 baud to see ping results.
   pinMode(inReleuPort1, OUTPUT);
   pinMode(13, OUTPUT);
}

////////////////////////////////////////////////     LOOP      /////////////////////////////////////////////
void loop() { 
   digitalWrite(inReleuPort1, HIGH);
   
 if ( pingDistanceNow < 20) {
           Serial.print(pingDistanceNow);
           Serial.println(" STOP!"); 
           digitalWrite(inReleuPort1, LOW); //START BREAK 
           delay(1000);  
      }
 if ( pingDistanceNow < 50 && pingDistanceNow > 19 ) {
           digitalWrite(inReleuPort1, HIGH);
           Serial.print(pingDistanceNow);
           Serial.println(" WARNING !");
           delay(5000); 
      }
 else {  
           digitalWrite(inReleuPort1, HIGH); // RELEASE BREAK
           digitalWrite(13, LOW); // RELEASE WARNING  
      }
      
}

