#include "TaskCNC.h"

void InitCNC(CNC* cnc)
{
  cnc->state = 0; // state disconect with GUI
  cnc->mode = 0; // mode idle

  cnc->btnOK = GPIO_PIN_4;
  cnc->btnExit = GPIO_PIN_5;
  cnc->btnUp = GPIO_PIN_3;
  cnc->btnDown = GPIO_PIN_15;

  cnc->Led = GPIO_PIN_0;
  cnc->Buzzer = GPIO_PIN_1; 

  cnc->uart.index = 0;
     
}

void ProcessBtnPress(CNC *cnc, osSemaphoreId xSemaphore)
{
  if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
  {
    for(;;)
    {
      if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
      {
        switch (cnc->state)
        {
        case 2: // mode error connect
          if(cnc-> btnPress == 1)// press OK button
          {
            HAL_GPIO_WritePin(GPIOB, cnc->Led, GPIO_PIN_SET);
            HAL_GPIO_WritePin(GPIOB, cnc->Buzzer, GPIO_PIN_RESET);
            cnc->state = 0;
          }
          break;
        
        default:
          break;
        }

      }
    }
  }
}

void ProcessMode(CNC *cnc, osSemaphoreId xSemaphore)
{
  if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
  {
    for(;;)
    {
      if(osSemaphoreWait(xSemaphore, osWaitForever) == osOK)
      {
        switch (cnc->mode)
        {
        case 3: // mode home
          sprintf(cnc->uart.SendToControl, "H.");
          HAL_UART_Transmit(cnc->uart.huart, cnc->uart.SendToControl, sizeof(cnc->uart.SendToControl), 100);
          break;

        case 4: // mode running

          break;
        
        case 5: // mode receive data

          break;

        default:
          break;
        }
      }
    }
  }
}

void ReceiveDataFromGUI(CNC *cnc, USBD_HandleTypeDef * husbd, osSemaphoreId xSemaphore, osSemaphoreId xSemaphoreMode)
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
            cnc->state = 1; // mode connect with GUI
            sprintf(cnc->DataSendToGUI, "C CONNECTED ");
            break;

          case '1': // disconnected
            cnc->state = 0; // mode disconect with GUI
            sprintf(cnc->DataSendToGUI, "C DISCONNECTED ");
            break;

          case '3': // home
            cnc->mode = 3; // mode home
            sprintf(cnc->DataSendToGUI, "C DOING ");
            osSemaphoreRelease(xSemaphoreMode);
            break;

          case '4': // start
            cnc->mode = 4; // mode running
            break;
          
          case '5': // receive data
            cnc->mode = 5; // mode receive data
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