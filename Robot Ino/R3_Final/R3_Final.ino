#include <Servo.h>  // add the servo libraries
//Create Servo objects to controll the servos.
Servo baseServo;
Servo upperArmServo;
Servo lowerArmServo;
Servo clawServo;

// Pin constants
const int RIGHT_X_PIN = A2;
const int RIGHT_Y_PIN = A5;
const int RIGHT_KEY_PIN = 7;
const int LEFT_X_PIN = A3;
const int LEFT_Y_PIN = A4;
const int LEFT_KEY_PIN = 8;
const int BASE_SERVO_PIN = A1;
const int UPPER_ARM_SERVO_PIN = A0;
const int LOWER_ARM_SERVO_PIN = 6;
const int CLAW_SERVO_PIN = 9;


// Servo angles
const int BASE_ANGLE_INITIAL = 90;
const int UPPER_ARM_ANGLE_INITIAL = 0;
const int LOWER_ARM_ANGLE_INITIAL = 129;
const int CLAW_ANGLE_INITIAL = 0;
const int BASE_ANGLE_MIN = 45;
const int BASE_ANGLE_MAX = 135;
const int UPPER_ARM_ANGLE_MIN = 0;
const int UPPER_ARM_ANGLE_MAX = 80;
const int LOWER_ARM_ANGLE_MIN = 35;
const int LOWER_ARM_ANGLE_MAX = 145;
const int CLAW_ANGLE_MIN = 0;
const int CLAW_ANGLE_MAX = 180;

// Joystick values
int rightX, rightY, rightKey;
int leftX, leftY, leftKey;

// Servo angles
int baseAngle = BASE_ANGLE_INITIAL;
int upperArmAngle = UPPER_ARM_ANGLE_INITIAL;
int lowerArmAngle = LOWER_ARM_ANGLE_INITIAL;
int clawAngle = CLAW_ANGLE_INITIAL;

// Timing variables
unsigned long previousMillis = 0;
unsigned long currentMillis = 0;
const int interval = 1000;
int increment = 1;
bool slowTurn = false;
bool goLeftXmax = false;
bool goLeftYmax = false;
bool goRightXmax = false;
bool goRightYmax = false;
bool goLeftXmin = false;
bool goLeftYmin = false;
bool goRightXmin = false;
bool goRightYmin = false;

// Parse incoming data
const int NUM_VALUES = 4;
byte buffer[256];
int values[NUM_VALUES];

void setup() {
  // boot posture
  baseServo.attach(BASE_SERVO_PIN);
  baseServo.write(baseAngle);
  upperArmServo.attach(UPPER_ARM_SERVO_PIN);
  upperArmServo.write(upperArmAngle);
  lowerArmServo.attach(LOWER_ARM_SERVO_PIN);
  lowerArmServo.write(lowerArmAngle);
  clawServo.attach(CLAW_SERVO_PIN);
  clawServo.write(clawAngle);

  pinMode(RIGHT_KEY_PIN, INPUT);  // set the right/left key to INPUT
  pinMode(LEFT_KEY_PIN, INPUT);
  Serial.begin(9600);  //  set the baud rate to 9600
}

void loop() {

  rightX = analogRead(RIGHT_X_PIN);
  rightY = analogRead(RIGHT_Y_PIN);
  rightKey = digitalRead(RIGHT_KEY_PIN);

  leftX = analogRead(LEFT_X_PIN);
  leftY = analogRead(LEFT_Y_PIN);
  leftKey = digitalRead(LEFT_KEY_PIN);

  // rotate
  zhuandong();
  // upper arm
  xiaobi();
  //  //lower arm
  dabi();
  //  // claw
  zhuazi();

  readPosition();

  serialEvent();
}

void readPosition() {

  currentMillis = millis();

  // check to see if the interval time is passed.
  if (currentMillis - previousMillis >= interval) {
    //print to serial monitor to see number results
    Serial.print("$");
    Serial.print(baseServo.read());
    Serial.print("#");
    Serial.print(upperArmServo.read());
    Serial.print("#");
    Serial.print(lowerArmServo.read());
    Serial.print("#");
    Serial.print(clawServo.read());
    Serial.println("$");
    // save the time when we displayed the string for the last time
    previousMillis = currentMillis;
  }
}

//******************************************************
// turn
void zhuandong() {
  if (rightX < 50 || goRightXmin)  // if push the right joystick to the right
  {
    baseAngle--;                     //baseAngle subtracts 1
    if (baseAngle < BASE_ANGLE_MIN)  // limit the angle when turn right
    {
      baseAngle = BASE_ANGLE_MIN;
    }
    delay(7.5);
  }
  if (rightX > 1000 || goRightXmax)  // if push the right joystick to the left
  {
    baseAngle++;                     //baseAngle plus 1
    if (baseAngle > BASE_ANGLE_MAX)  // limit the angle when turn left
    {
      baseAngle = BASE_ANGLE_MAX;
    }
    delay(7.5);
  }
  if (slowTurn) { delay(2.5); }
  baseServo.write(baseAngle);
}

//**********************************************************/
////upper arm
void xiaobi() {
  if (rightY > 1000 || goRightYmax)  // if push the right joystick upward
  {
    upperArmAngle--;
    if (upperArmAngle < UPPER_ARM_ANGLE_MIN)  // limit the lifting angle
    {
      upperArmAngle = UPPER_ARM_ANGLE_MIN;
    }
    delay(7.5);
  }
  if (rightY < 50 || goRightYmin)  // if push the right joystick downward
  {
    upperArmAngle++;
    if (upperArmAngle > UPPER_ARM_ANGLE_MAX)  // limit the angle when go down
    {
      upperArmAngle = UPPER_ARM_ANGLE_MAX;
    }
    delay(7.5);
  }
  if (slowTurn) { delay(1); }
  upperArmServo.write(upperArmAngle);
}
//*************************************************************
// lower arm
void dabi() {
  if (leftY < 50 || goLeftYmin)  // if push the left joystick upward
  {
    lowerArmAngle++;
    if (lowerArmAngle > LOWER_ARM_ANGLE_MAX)  // limit the stretched angle
    {
      lowerArmAngle = LOWER_ARM_ANGLE_MAX;
    }
    delay(7.5);
  }
  if (leftY > 1000 || goLeftYmax)  // if push the left joystick downward
  {
    lowerArmAngle--;
    if (lowerArmAngle < LOWER_ARM_ANGLE_MIN)  // limit the retracted angle
    {
      lowerArmAngle = LOWER_ARM_ANGLE_MIN;
    }
    delay(7.5);
  }
  if (slowTurn) { delay(1); }
  lowerArmServo.write(lowerArmAngle);
}
//*************************************************************
//claw
void zhuazi() {
  //claw
  if (leftX < 50 || goLeftXmin)  // if push the left joystick to the right
  {
    clawAngle--;
    if (clawAngle < CLAW_ANGLE_MIN)  // if pos4 value subtracts to 2, the claw in 37 degrees we have tested is closed.
    {                                //（should change the value based on the fact）
      clawAngle = CLAW_ANGLE_MIN;    // stop subtraction when reduce to 2
    }
    delay(3);
  }
  if (leftX > 1000 || goLeftXmax)  //// if push the left joystick to the left
  {
    clawAngle++;                     // current angle of servo 4 plus 8（change the value you plus, thus change the open speed of claw）
    if (clawAngle > CLAW_ANGLE_MAX)  // limit the largest angle when open the claw
    {
      clawAngle = CLAW_ANGLE_MAX;
    }
    delay(3);
  }
  if (slowTurn) { delay(1); }
  clawServo.write(clawAngle);
}
String commandBefore = "";
String command = "";
//*************************************************************
void serialEvent() {


  if (Serial.available()) {
    int bytesRead = Serial.readBytesUntil('!', buffer, sizeof(buffer));  // read until '^'
    buffer[bytesRead] = '\0';                                            // add null terminator
    Serial.println(buffer[bytesRead]);
    String mystring = (char *)buffer;
    if (mystring.indexOf('@') != -1) {
      slowTurn = true;
      char *valueString = strtok((char *)buffer, "@");  // split into values using '@' delimiter
      int i = 0;
      while (valueString != NULL && i < NUM_VALUES) {
        values[i++] = atoi(valueString);  // convert string to integer and store in array
        valueString = strtok(NULL, "@");  // get next value
      }
      // assign values to variables as desired

      baseAngle = values[0];
      upperArmAngle = values[1];
      lowerArmAngle = values[2];
      clawAngle = values[3];
    } else {

      slowTurn = false;
      Serial.println(mystring);
      command = mystring;
      // process the command
      if (command == "left") {
        if (baseAngle == 90) {
          while (baseAngle != 135) {
            goRightXmax = true;
            loop();
          }
          goRightXmax = false;
        }
        if (baseAngle == 45) {
          while (baseAngle != 135) {
            goRightXmax = true;
            loop();
          }
          goRightXmax = false;
        }

      } else if (command == "right") {
        if (baseAngle == 90) {
          while (baseAngle != 45) {
            goRightXmin = true;
            loop();
          }
          goRightXmin = false;
        }
        if (baseAngle == 135) {
          while (baseAngle != 45) {
            goRightXmin = true;
            loop();
          }
          goRightXmin = false;
        }

      } else if (command == "front") {
        if (lowerArmAngle == 129) {
          while (lowerArmAngle != 65) {
            goLeftYmax = true;
            loop();
          }
          goLeftYmax = false;
        }
        if (upperArmAngle == 0) {
          while (upperArmAngle != 50) {
            goRightYmin = true;
            loop();
          }
          goRightYmin = false;
        }
      } else if (command == "open") {
        if (clawAngle == 0) {
          while (clawAngle != 115) {
            goLeftXmax = true;
            loop();
          }
          goLeftXmax = false;
        }
      } else if (command == "grab") {

        if (clawAngle == 115) {
          while (clawAngle != 0) {
            goLeftXmin = true;
            loop();
          }
          goLeftXmin = false;
        }
      } else if (command == "home") {
        if (baseAngle == 135) {
          while (baseAngle != 90) {
            goRightXmin = true;
            loop();
          }
          goRightXmin = false;
        }
        if (baseAngle == 45) {
          while (baseAngle != 90) {
            goRightXmax = true;
            loop();
          }
          goRightXmax = false;
        }

        if (lowerArmAngle == 65) {
          while (lowerArmAngle != 129) {
            goLeftYmin = true;
            loop();
          }
          goLeftYmin = false;
        }
        if (upperArmAngle == 50) {
          while (upperArmAngle != 0) {
            goRightYmax = true;
            loop();
          }
          goRightYmax = false;
        }
        /*if(clawAngle==115)
        {while(clawAngle!=0)
          {goLeftXmin = true; loop();}
          goLeftXmin=false;
        }*/
      }
    }
  }
}
