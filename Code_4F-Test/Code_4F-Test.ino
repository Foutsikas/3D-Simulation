#include <Servo.h>  // add the servo libraries
Servo myservo1;  // create servo object to control a servo
Servo myservo2;
Servo myservo3;
Servo myservo4;

const int pos1_initial = 90;
const int pos2_initial = 0;
const int pos3_initial = 80;
const int pos4_initial = 0;

int pos1 = pos1_initial;
int pos2 = pos2_initial;
int pos3 = pos3_initial;
int pos4 = pos4_initial;

const int right_X = A2; // define the right X pin to A2
const int right_Y = A5; // define the right Y pin to A5
const int right_key = 7; // define the right key pin to 7（that is the value of Z）

const int left_X = A3; // define the left X pin to A3
const int left_Y = A4;  // define the left X pin to A4
const int left_key = 8; //define the left key pin to 8（that is the value of Z）

int x1, y1, z1; // define the variable, used to save the joystick value it read.
int x2, y2, z2;

unsigned long previousMillis  = 0;
unsigned long currentMillis   = 0;
const int interval = 1000;

const int numValues = 4; // number of values to receive
byte buffer[256]; // buffer to hold incoming data
int values[numValues]; // array to hold parsed values

void setup()
{
  // boot posture
  myservo1.attach(A1);  // set the control pin of servo 1 to A1
  myservo1.write(pos1_initial);
  myservo2.attach(A0);  // set the control pin of servo 2 to A0
  myservo2.write(pos2_initial);
  myservo3.attach(6);   //set the control pin of servo 3 to D6
  myservo3.write(pos3_initial);
  myservo4.attach(9);   // set the control pin of servo 4 to D9
  myservo4.write(pos4_initial);

  pinMode(right_key, INPUT);   // set the right/left key to INPUT
  pinMode(left_key, INPUT);
  Serial.begin(9600); //  set the baud rate to 9600
}

void loop()
{
  x1 = analogRead(right_X);
  y1 = analogRead(right_Y);
  z1 = digitalRead(right_key);

  x2 = analogRead(left_X);
  y2 = analogRead(left_Y);
  z2 = digitalRead(left_key);

  // rotate
  zhuandong();
  // upper arm
  xiaobi();
  //lower arm
  dabi();
  // claw
  zhuazi();
  readPosition();
}

void readPosition() {

  currentMillis = millis();

  // check to see if the interval time is passed.
  if (currentMillis - previousMillis >= interval) {
    //print to serial monitor to see number results
    Serial.print("$");
    Serial.print(myservo1.read());
    Serial.print("#");
    Serial.print(myservo2.read());
    Serial.print("#");
    Serial.print(myservo3.read());
    Serial.print("#");
    Serial.print(myservo4.read());
    Serial.println("$");
    // save the time when we displayed the string for the last time
    previousMillis = currentMillis;
  }
}

//******************************************************
// turn
void zhuandong()
{
  if (x1 < 50) // if push the right joystick to the right
  {
    pos1--; //pos1 subtracts 1
    if (pos1 < 46) // limit the angle when turn right
    {
      pos1 = 45;
    }
  }
  if (x1 > 1000) // if push the right joystick to the let
  {
    pos1++; //pos1 plus 1
    if (pos1 > 134) // limit the angle when turn left
    {
      pos1 = 135;
    }
  }
  myservo1.write(pos1);
  delay(2.5);
}

//**********************************************************/
//upper arm
void xiaobi()
{
  if (y1 > 1000) // if push the right joystick upward
  {
    pos2--;
    if (pos2 < 0) // limit the lifting angle
    {
      pos2 = 0;
    }
  }
  if (y1 < 50) // if push the right joystick downward
  {
    pos2++;
    if (pos2 > 79) // limit the angle when go down
    {
      pos2 = 80;
    }
  }
  myservo2.write(pos2);
  delay(1);
}

//*************************************************************/
// lower arm
void dabi()
{
  if (y2 < 50) // if push the left joystick upward
  {
    pos3++;
    if (pos3 > 144) // limit the stretched angle
    {
      pos3 = 145;
    }
  }
  if (y2 > 1000) // if push the left joystick downward
  {
    pos3--;
    if (pos3 < 36) // limit the retracted angle
    {
      pos3 = 35;
    }
  }
  myservo3.write(pos3);
  delay(1);
}
//*************************************************************/
//claw
void zhuazi()
{
  //claw
  if (x2 < 50) // if push the left joystick to the right
  {
    pos4--;
    if (pos4 < 2) // if pos4 value subtracts to 2, the claw in 37 degrees we have tested is closed.
    { //（should change the value based on the fact）
      pos4 = 0; // stop subtraction when reduce to 2
    }
  }
  if (x2 > 1000) //// if push the left joystick to the left
  {
    pos4++; // current angle of servo 4 plus 8（change the value you plus, thus change the open speed of claw）
    if (pos4 > 107) // limit the largest angle when open the claw
    {
      pos4 = 60;
    }
  }
  myservo4.write(pos4);
  delay(2.5);
}
//*************************************************************/
//Testing/
void serialEvent()
{
  if (Serial.available())
  {
    int bytesRead = Serial.readBytesUntil('!', buffer, sizeof(buffer)); // read until '^'
    buffer[bytesRead] = '\0'; // add null terminator
    Serial.println(buffer[bytesRead]);
    char *valueString = strtok((char*)buffer, "@"); // split into values using '@' delimiter
    int i = 0;
    while (valueString != NULL && i < numValues)
    {
      values[i++] = atoi(valueString); // convert string to integer and store in array
      valueString = strtok(NULL, "@"); // get next value
    }
    // assign values to variables as desired
    pos1 = values[0];
    pos2 = values[1];
    pos3 = values[2];
    pos4 = values[3];
    myservo1.write(pos1);
    myservo2.write(pos2);
    myservo3.write(pos3);
    myservo4.write(pos4);
    Serial.write(myservo1.read());
    Serial.write(myservo2.read());
    Serial.write(myservo3.read());
    Serial.write(myservo4.read());
  }
}
