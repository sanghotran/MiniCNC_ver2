#include "TaskCNC.h"

void InitCNC(CNC* cnc)
{
  cnc->mode = 0; // mode disconect with GUI

  cnc->btnOK = GPIO_PIN_4;
  cnc->btnExit = GPIO_PIN_5;
  cnc->btnUp = GPIO_PIN_3;
  cnc->btnDown = GPIO_PIN_15;

  cnc->Led = GPIO_PIN_0;
  cnc->Buzzer = GPIO_PIN_1; 

    
}

void ProcessBtnPress(CNC *cnc, osSemaphoreId xSemaphore)
{
  if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
  {
    for(;;)
    {
      if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
      {
        switch (cnc->mode)
        {
        case 3: // mode error connect
          if(cnc-> btnPress == 1)// press OK button
          {
            HAL_GPIO_WritePin(GPIOB, cnc->Led, GPIO_PIN_SET);
            HAL_GPIO_WritePin(GPIOB, cnc->Buzzer, GPIO_PIN_RESET);
            cnc->mode = 0;
          }
          break;
        
        default:
          break;
        }

      }
    }
  }
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
        case 'C': // command          
          switch (cnc->DataReceiveFromGUI[2])
          {
          case '0': // connected
            cnc->mode = 1; // mode connect with GUI
            sprintf(cnc->DataSendToGUI, "C CONNECTED ");
            break;
          case '1': // disconnected
            cnc->mode = 0; // mode disconect with GUI
            sprintf(cnc->DataSendToGUI, "C DISCONNECTED ");
            break;
          default:
            break;
          }
          
          break;
        case 'D': // data

          break;
        default:
          return;
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