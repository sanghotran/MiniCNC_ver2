#include "TaskCNC.h"

void InitCNC(CNC* cnc)
{
  cnc->enbCheckConnect = false;

  cnc->btnOK = GPIO_PIN_4;
  cnc->btnExit = GPIO_PIN_5;
  cnc->btnUp = GPIO_PIN_3;
  cnc->btnDown = GPIO_PIN_15;

  cnc->Led = GPIO_PIN_0;
  cnc->Buzzer = GPIO_PIN_1; 

    
}

void ReceiveDataFromGUI(CNC *cnc, USBD_HandleTypeDef * husbd, osSemaphoreId xSemaphore)
{
  // vì lúc đầu semaphore không được là 0 nên phải lấy đi 1 ngay chỗ này để không chạy func này khi mới vào
  if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
  {
    for(;;)
    {
      if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
      {
        switch (cnc->DataReceiveFromGUI[0])
        {
        case '0': // check connect
          sprintf(cnc->DataSendToGUI, "Haa");
          cnc->enbCheckConnect = true;
          break;
          
        default:
          break;
        }
        USBD_CUSTOM_HID_SendReport(husbd, (uint8_t*)cnc->DataSendToGUI, 65);     
      }        
    }
  }  
}

void ReceiveDataFromCNC(void)
{

}