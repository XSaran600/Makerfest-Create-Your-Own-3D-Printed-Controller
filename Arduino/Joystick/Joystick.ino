// Button
const int Jump_pin = 2; // digital pin connected to switch output
const int Sprint_pin = 3; // digital pin connected to switch output

// Joystick
const int X_pin = 0; // analog pin connected to X output
const int Y_pin = 1; // analog pin connected to Y output

// Lightsensor
//int lightSensorPin = 2; // analog pin connected to lightsensor

// Temp bools
bool jump = false;
bool sprint = false;

void setup() {
  pinMode(Jump_pin, INPUT);
  digitalWrite(Jump_pin, HIGH);
  pinMode(Sprint_pin, INPUT);
  digitalWrite(Sprint_pin, HIGH);
  Serial.begin(9600);
}

void loop() {
  
  //Serial.print("Switch:  ");
  //Serial.print(digitalRead(SW_pin));
  //Serial.print("\n");
  /*
  Serial.print("X-axis: ");
  Serial.print(analogRead(X_pin));
  Serial.print("\n");
  Serial.print("Y-axis: ");
  Serial.println(analogRead(Y_pin));
  Serial.print("\n\n");
  */
  /*
  Serial.println(analogRead(lightSensorPin));
  */
  /*
    Serial.print("Jump:  ");
    Serial.println(digitalRead(Jump_pin));
    Serial.print("\n");

    Serial.print("Sprint:  ");
    Serial.println(digitalRead(Sprint_pin));
    Serial.print("\n");
    */

  // Move
  if(analogRead(Y_pin) > 700) // Joystick Right
  {
    Serial.write(2);
    Serial.flush();
  }
  else if(analogRead(Y_pin) < 400) // Joystick Left
  {
    Serial.write(3);
    Serial.flush();
  }
  else // Joystick still
  {
    Serial.write(1);
    Serial.flush();
  }
/*
  // Lightsensor
  if(analogRead(lightSensorPin) < 100) // Light sensor blocked
  {
    Serial.write(6);
    Serial.flush();
  }
*/

  // Jump
  if(digitalRead(Jump_pin) == 0) // Button not pressed
  {
    if (jump == true)
    {
      Serial.write(4);
      Serial.flush();
      jump = false;
    }
  }
  if(digitalRead(Jump_pin) == 1) // Button pressed
  {
    if (jump == false)
    {
      Serial.write(5);
      Serial.flush();
      jump = true;
    }
  }

  // Sprint
  if(digitalRead(Sprint_pin) == 0) // Button not pressed
  {
    if (sprint == false)
    {
      Serial.write(7);
      Serial.flush();
      sprint = true;
    }
  }
  if(digitalRead(Sprint_pin) == 1) // Button pressed
  {
    if (sprint == true)
    {
      Serial.write(8);
      Serial.flush();
      sprint = false;
    }
  }
  
  delay(100);
}
