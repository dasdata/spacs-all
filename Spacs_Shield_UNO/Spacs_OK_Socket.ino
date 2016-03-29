/*
 * UIPEthernet EchoServer example.
 *
 * UIPEthernet is a TCP/IP stack that can be used with a enc28j60 based
 * Ethernet-shield.
 *
 *  This is a demo of SPACS user running as webserver with the Ether Card
 *  2016-03-10 <md@dasdata.co> http://dasdata.co/spacs
 */

#include <UIPEthernet.h>

EthernetServer server = EthernetServer(27015);

/*   
  =======  HMC5883L compass  =========
 * SCL connection of the sensor attached to analog pin A5
 * SDA connection of the sensor attached to analog pin A4
 * GND connection of the sensor attached to ground
 * VCC connection of the sensor attached to +5V
*/
#include <Wire.h>
#include <HMC5883L.h>
HMC5883L compass;

/*  
 =======  SONAR - HC-SR04 ultrasonic rangefinder =========
 * ECHO connection of the sensor attached to digital pin 6
 * TRIG connection of the sensor attached to digital pin 7
 * GND connection of the sensor attached to ground
 * VCC connection of the sensor attached to +5V
*/
const int echoPin = 6;
const int trigPin = 7;
const int maxDist = 200; // maximum distance 
char SpcUserID[15]  = "S1234"; //device unique ID 

void setup()
{
  Serial.begin(9600);

  // Init compas 
 Wire.begin();
 compass = HMC5883L();
 compass.SetScale(1.3);
 compass.SetMeasurementMode(Measurement_Continuous);
 
// eth
  uint8_t mac[6] = {0x00,0x01,0x02,0x03,0x04,0x05};
  IPAddress myIP(192,168,0,109); 
  Ethernet.begin(mac,myIP);

  server.begin(); 
}

void loop()
{
  // get compass values 
 MagnetometerRaw raw = compass.ReadRawAxis();
 MagnetometerScaled scaled = compass.ReadScaledAxis();
 float xHeading = atan2(scaled.YAxis, scaled.XAxis);
 float yHeading = atan2(scaled.ZAxis, scaled.XAxis);
 float zHeading = atan2(scaled.ZAxis, scaled.YAxis);
 if(xHeading < 0) xHeading += 2*PI;
 if(xHeading > 2*PI) xHeading -= 2*PI;
 if(yHeading < 0) yHeading += 2*PI; 
 if(yHeading > 2*PI) yHeading -= 2*PI;
 if(zHeading < 0) zHeading += 2*PI;
 if(zHeading > 2*PI) zHeading -= 2*PI;
 float xDegrees = xHeading * 180/M_PI;
 float yDegrees = yHeading * 180/M_PI;
 float zDegrees = zHeading * 180/M_PI;

  // establish variables for duration of the ping, 
  // and the distance result in inches and centimeters:
  long duration, inches, cm;

  // The sensor is triggered by a HIGH pulse of 10 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
  pinMode(trigPin, OUTPUT);
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(5);
  digitalWrite(trigPin, LOW);

  // Read the signal from the sensor: a HIGH pulse whose
  // duration is the time (in microseconds) from the sending
  // of the ping to the reception of its echo off of an object.
  pinMode(echoPin, INPUT);
  duration = pulseIn(echoPin, HIGH);

  // convert the time into a distance
  inches = microsecondsToInches(duration);
  cm = microsecondsToCentimeters(duration);

if (cm< maxDist) { 
//  Serial.print("User:");
  Serial.print(SpcUserID); 
    Serial.print(" ");
    
  Serial.print(cm);
  Serial.print(" cm ");
  
   Serial.print("x:");
    Serial.print(xDegrees);

   Serial.print(" y:");
    Serial.print(yDegrees);

    Serial.print(" z:");
    Serial.print(zDegrees);
 
  Serial.println();
}

  
  size_t size;

  if (EthernetClient client = server.available())
    {
      while((size = client.available()) > 0)
        {
          uint8_t* msg = (uint8_t*)malloc(size);
          size = client.read(msg,size);
          Serial.write(msg,size);
          free(msg);
        }
      client.print(cm);  client.print(" "); 
      client.print(xDegrees);  client.print(" "); 
      client.print(yDegrees);  client.print(" "); 
      client.println(zDegrees); client.println(""); 
      client.stop();
    }

delay (2000);
    
}



/*  ===================         CONVERSIONS          =========================== */
long microsecondsToInches(long microseconds)
{
  // According to Parallax's datasheet for the PING))), there are
  // 73.746 microseconds per inch (i.e. sound travels at 1130 feet per
  // second).  This gives the distance travelled by the ping, outbound
  // and return, so we divide by 2 to get the distance of the obstacle.
  // See: http://www.parallax.com/dl/docs/prod/acc/28015-PING-v1.3.pdf
  return microseconds / 74 / 2;
}

long microsecondsToCentimeters(long microseconds)
{
  // The speed of sound is 340 m/s or 29 microseconds per centimeter.
  // The ping travels out and back, so to find the distance of the
  // object we take half of the distance travelled.
  return microseconds / 29 / 2;
}



