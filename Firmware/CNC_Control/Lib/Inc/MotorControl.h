#ifndef __MOTORCONTROL_H
#define __MOTORCONTROL_H

#include <stdbool.h>
#include "MyMath.h"
#include "MyStruct.h"
#include "stm32f1xx_hal.h"
#include <math.h>
#include <stdio.h>

#define PI 3.1415926535897932384626433832795
#define HALF_PI 1.5707963267948966192313216916398
#define TWO_PI 6.283185307179586476925286766559
#define DEG_TO_RAD 0.017453292519943295769236907684886
#define RAD_TO_DEG 57.295779513082320876798154814105

#define TS 0.02

#define MAX_SPEED 60

//#define ERROR 5

#define STEP 0.01
	
#define LINE_MAX 2 // max line lengh (mm) when draw circle

void ProcessData(CNC*);

void PWM(AXIS *axis);

//void readEncoder(TIM_HandleTypeDef* htim, int *Pos);

void readEncoder(AXIS *axis);

void sample(AXIS *axis);

void PID_control(int sp, AXIS *pid);

void HOME(AXIS *axis);

void move(AXIS *axis, float pos);

void moveGcode(AXIS *pAxis);

void drawLine(AXIS *pXAxis, AXIS *pYAxis);

void runDrill(DRILL *drill, float pwm);



#endif /* __MOTORCONTROL_H */
