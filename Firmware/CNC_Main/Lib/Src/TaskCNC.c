#include "TaskCNC.h"

int bufsize (char *buf)
{
	int i=0;
	while (*buf++ != '\0') i++;
	return i;
}

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

  cnc->home = 0;

  //cnc->sd.fresult = f_mount(cnc->sd.FileSystem, "/", 1);
     
}

void SaveDataToSD(CNC *cnc)
{
  //cnc->sd.fresult = f_open(cnc->sd.File, cnc->sd.FileName, FA_OPEN_ALWAYS | FA_READ | FA_WRITE);

  	// Move to offset to the end of the file 
  	//cnc->sd.fresult = f_lseek(cnc->sd.File, f_size(cnc->sd.File)); 

  	// write the string to the file 
  	//cnc->sd.fresult = f_puts(cnc->sd.data, cnc->sd.File);

  	//f_close (&fil);
}

void ProcessBtnPress(CNC *cnc)
{  
  switch (cnc->state)
  {
    case 2: // state error connect
      if(cnc-> btnPress == 1)// press OK button
      {
        HAL_GPIO_WritePin(GPIOB, cnc->Led, GPIO_PIN_RESET);
        HAL_GPIO_WritePin(GPIOB, cnc->Buzzer, GPIO_PIN_RESET);
        cnc->state = 0;
      }
      break;
        
    default:
      break;
  }
}

void ProcessMode(CNC *cnc)
{
  switch (cnc->mode)
  {
    case 3: // mode home
      memset(cnc->uart.SendToControl, 0, sizeof(cnc->uart.SendToControl));
      sprintf(cnc->uart.SendToControl, "H!");
      HAL_UART_Transmit(cnc->uart.huart, cnc->uart.SendToControl, 2, 100);
      break;

    case 4: // mode running

      break;
          
    case 5: // mode receive file name of gcode
      //sscanf(cnc->DataReceiveFromGUI, "C 5 %s", cnc->sd.FileName);
      break;

    case 6: // mode receive data of gcode
      memset(cnc->uart.SendToControl, 0, sizeof(cnc->uart.SendToControl));
      sscanf(cnc->DataReceiveFromGUI, "D 1 %s", cnc->uart.SendToControl);
      HAL_UART_Transmit(cnc->uart.huart, cnc->uart.SendToControl, sizeof(cnc->uart.SendToControl), 100);
      //SaveDataToSD(cnc);
      //sprintf(cnc->DataSendToGUI, "C ACK ");
      //USBD_CUSTOM_HID_SendReport(cnc->husb, (uint8_t*)cnc->DataSendToGUI, sizeof(cnc->DataSendToGUI));
      
      break;

    default:
      break;
  } 
}


void ReceiveDataFromGUI(CNC *cnc, SemaphoreHandle_t xSemaphoreMode)
{
  switch (cnc->DataReceiveFromGUI[0])
  {
    case 'C': // command          
      switch (cnc->DataReceiveFromGUI[2])
      {
        case '0': // connected
          cnc->state = 1; // mode connect with GUI
          cnc->mode = 0; // reset mode when connect
          sprintf(cnc->DataSendToGUI, "C CONNECTED ");
          break;

        case '1': // disconnected
          cnc->state = 0; // mode disconect with GUI
          sprintf(cnc->DataSendToGUI, "C DISCONNECTED ");
          break;

        case '3': // home
          cnc->mode = 3; // mode home
          sprintf(cnc->DataSendToGUI, "C DOING ");
          break;

        case '4': // start
          if( cnc->home == 1)
          {
            cnc->mode = 4; // mode running
            sprintf(cnc->DataSendToGUI, "C RUNNING ");
          }
          else
          {
            sprintf(cnc->DataSendToGUI, "C NOHOME ");
          }
          break;
          
        case '5': // receive file name of gcode
          cnc->mode = 5; // mode receive file name of gcode
          sprintf(cnc->DataSendToGUI, "C YES ");
          break;

        default:
          break;
      }     
      break;
    case 'D': // data
      if(cnc->DataReceiveFromGUI[2] == '0')
      {
        sprintf(cnc->DataSendToGUI, "C DONE ");
        //f_close (cnc->sd.File);
        cnc->mode = 0;
      }
      else
      {
        cnc->mode = 6; // mode receive data of gcode
      }          
      break;

    default:
      return;
  }
  USBD_CUSTOM_HID_SendReport(cnc->husb, (uint8_t*)cnc->DataSendToGUI, sizeof(cnc->DataSendToGUI));
  if(cnc->mode > 2)   
    xSemaphoreGive(xSemaphoreMode);
}

void ReceiveDataFromCNC(CNC *cnc)
{
  switch (cnc->uart.ReceiveFromControl[0])
	{
	case 'H':
		sprintf(cnc->DataSendToGUI, "C HOME ");
    cnc->home = 1; // have just come home
		break;
	case 'G':
		sprintf(cnc->DataSendToGUI, "C ACK %s ", cnc->uart.ReceiveFromControl);
		break;	
	default:
		return;
  }
  memset(cnc->uart.ReceiveFromControl, 0, sizeof(cnc->uart.ReceiveFromControl));
  cnc->uart.index = 0;
  USBD_CUSTOM_HID_SendReport(cnc->husb, (uint8_t*)cnc->DataSendToGUI, sizeof(cnc->DataSendToGUI));
}