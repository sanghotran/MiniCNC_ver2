#ifndef  __TASKCNC_H
#define __TASKCNC_H

#include "main.h"
#include <stdbool.h>
#include "usb_device.h"
#include "usbd_customhid.h"
#include "FreeRTOS.h"
#include "task.h"
#include "semphr.h"

typedef struct
{
    UART_HandleTypeDef *huart;
    uint8_t SendToControl[15];
    char Receive;
    uint8_t index;
    char ReceiveFromControl[15];
} UART;

// typedef struct
// {
//     FATFS *FileSystem;
//     FIL *File;
//     FRESULT fresult;
//     char data[64];
//     char FileName[15];

//     int br, bw;

//     FATFS *pfs;
//     DWORD fre_clust;
//     uint32_t total, free_space;
// } SD;


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
    char DataSendToGUI[19];
    USBD_HandleTypeDef *husb;

    uint8_t state;
    uint8_t mode;
    uint8_t home;
   
    UART uart;

    //SD sd;

} CNC;

void InitCNC(CNC*);

void SaveDataToSD(CNC*);

void ProcessBtnPress(CNC*);

void ProcessMode(CNC*);

void ReceiveDataFromGUI(CNC*, SemaphoreHandle_t);

void ReceiveDataFromCNC(CNC*);


#endif