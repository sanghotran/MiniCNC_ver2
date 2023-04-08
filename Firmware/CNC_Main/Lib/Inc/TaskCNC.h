#ifndef  __TASKCNC_H
#define __TASKCNC_H

#include "main.h"
#include "cmsis_os.h"
#include <stdbool.h>
#include "usb_device.h"
#include "usbd_customhid.h"

typedef struct
{
    uint16_t btnOK;
    uint16_t btnExit;
    uint16_t btnUp;
    uint16_t btnDown;

    uint8_t btnPress;

    uint16_t Led;
    uint16_t Buzzer;

    char DataReceiveFromGUI[64];
    char DataSendToGUI[65];

    uint8_t state;
    uint8_t mode;


    char Send[10];
    char Receive[10];



} CNC;

void InitCNC(CNC*);

void ProcessBtnPress(CNC*, osSemaphoreId);

void ProcessMode(CNC*, osSemaphoreId);

void ReceiveDataFromGUI(CNC*,USBD_HandleTypeDef*, osSemaphoreId, osSemaphoreId);

void ReceiveDataFromCNC(void);


#endif