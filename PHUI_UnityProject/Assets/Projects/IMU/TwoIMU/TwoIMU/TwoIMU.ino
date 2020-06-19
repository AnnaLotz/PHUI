#include "Uduino.h"  // Include Uduino library at the top of the sketch
Uduino uduino("TWO_IMU");

#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"


MPU6050 mpu1(0x68);
MPU6050 mpu2(0x69);

// mpu1 control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize1;    // expected DMP packet size (default is 42 bytes)
uint16_t packetSize2;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer


Quaternion q;    // We only get quaternion


void setup() {
  Wire.begin();
  Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties

  Serial.begin(38400);

  while (!Serial); // wait for Leonardo enumeration, others continue immediately

  mpu1.initialize();
  devStatus = mpu1.dmpInitialize();
  mpu1.setXGyroOffset(54); //++
  mpu1.setYGyroOffset(-21); //--
  mpu1.setZGyroOffset(5);

  if (devStatus == 0) {
    mpu1.setDMPEnabled(true);
    // set our DMP Ready flag so the main loop() function knows it's okay to use it
    // get expected DMP packet size for later comparison
    packetSize1 = mpu1.dmpGetFIFOPacketSize();

    //  Then start initializing mpu 2
    mpu2.initialize();
    devStatus = mpu2.dmpInitialize();
    mpu2.setXGyroOffset(54); //++
    mpu2.setYGyroOffset(-21); //--
    mpu2.setZGyroOffset(5);

    if (devStatus == 0) {
      mpu2.setDMPEnabled(true);
      // set our DMP Ready flag so the main loop() function knows it's okay to use it
      dmpReady = true;
      // get expected DMP packet size for later comparison
      packetSize2 = mpu2.dmpGetFIFOPacketSize();
    } else {
      // Error
      Serial.println("Error initializing MPU2");
    }


  } else {
    // Error
    Serial.println("Error initializing MPU1");
  }


}



void loop() {
  uduino.update();

  if (uduino.isInit()) {
    if (!dmpReady) {
      Serial.println("IMU not connected.");
      delay(10);
      return;
    }
    GetMPU1();
    GetMPU2();

  }
}

void GetMPU1() {
  int  mpu1IntStatus = mpu1.getIntStatus();
  fifoCount = mpu1.getFIFOCount();

  if ((mpu1IntStatus & 0x10) || fifoCount == 1024) { // check if overflow
    mpu1.resetFIFO();
  } else if (mpu1IntStatus & 0x02) {
    while (fifoCount < packetSize1) fifoCount = mpu1.getFIFOCount();

    mpu1.getFIFOBytes(fifoBuffer, packetSize1);
    fifoCount -= packetSize1;
  
    mpu1.dmpGetQuaternion(&q, fifoBuffer);
    Serial.print("r1/");
    Serial.print(q.w, 4); Serial.print("/");
    Serial.print(q.x, 4); Serial.print("/");
    Serial.print(q.y, 4); Serial.print("/");
    Serial.println(q.z, 4);
  }
}


void GetMPU2() {
  int  mpu2IntStatus = mpu2.getIntStatus();
  fifoCount = mpu2.getFIFOCount();

  if ((mpu2IntStatus & 0x10) || fifoCount == 1024) { // check if overflow
    mpu2.resetFIFO();
  } else if (mpu2IntStatus & 0x02) {
    while (fifoCount < packetSize2) fifoCount = mpu2.getFIFOCount();

    mpu2.getFIFOBytes(fifoBuffer, packetSize2);
    fifoCount -= packetSize2;
  
    mpu2.dmpGetQuaternion(&q, fifoBuffer);
    Serial.print("r2/");
    Serial.print(q.w, 4); Serial.print("/");
    Serial.print(q.x, 4); Serial.print("/");
    Serial.print(q.y, 4); Serial.print("/");
    Serial.println(q.z, 4);
  }
}
