#include "TaskCNC.h"

void InitCNC(CNC* cnc)
{
    cnc->btnOK = GPIO_PIN_4;
    cnc->btnExit = GPIO_PIN_5;
    cnc->btnUp = GPIO_PIN_3;
    cnc->btnDown = GPIO_PIN_15;

    cnc->Led = GPIO_PIN_0;
    cnc->Buzzer = GPIO_PIN_1; 

    
}

void ReceiveDataFromGUI(CNC *cnc, USBD_HandleTypeDef * husbd, osSemaphoreId xSemaphore)
{
    for(;;)
  {
    if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
    {
      switch (cnc->DataReceiveFromGUI[0])
      {
      case '0': // check connect
        sprintf(cnc->DataSendToGUI, "0");
        break;
      
      default:
        break;
      }
      //USBD_CUSTOM_HID_SendReport(husbd, (uint8_t*)cnc->DataSendToGUI, 64);     
    }
  }
}

void ReceiveDataFromCNC(void)
{

}