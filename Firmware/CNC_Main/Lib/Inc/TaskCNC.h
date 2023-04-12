#ifndef  __TASKCNC_H
#define __TASKCNC_H

#include "main.h"
#include "cmsis_os.h"
#include <stdbool.h>
#include "usb_device.h"
#include "usbd_customhid.h"
#include "fatfs.h"

typedef struct
{
    UART_HandleTypeDef *huart;
    uint8_t SendToControl[10];
    char Receive;
    uint8_t index;
    char ReceiveFromControl[10];
} UART;

typedef struct
{
    FATFS FileSystem;
    FIL File;
    FRESULT fresult;
    char data[1024];

    int br, bw;

    FATFS *pfs;
    DWORD fre_clust;
    uint32_t total, free_space;
} SD;


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
    char DataSendToGUI[15];

    uint8_t state;
    uint8_t mode;
   
    UART uart;

    SD sd;

} CNC;

void InitCNC(CNC*);

void ProcessBtnPress(CNC*, osSemaphoreId);

void ProcessMode(CNC*, osSemaphoreId);

void ReceiveDataFromGUI(CNC*,USBD_HandleTypeDef*, osSemaphoreId, osSemaphoreId);

void ReceiveDataFromCNC(void);


#endif