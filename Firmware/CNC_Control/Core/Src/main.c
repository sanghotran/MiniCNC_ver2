/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2023 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "MotorControl.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
TIM_HandleTypeDef htim1;
TIM_HandleTypeDef htim2;
TIM_HandleTypeDef htim3;
TIM_HandleTypeDef htim4;

UART_HandleTypeDef huart2;

/* USER CODE BEGIN PV */

CNC cnc;

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_TIM1_Init(void);
static void MX_TIM2_Init(void);
static void MX_TIM3_Init(void);
static void MX_TIM4_Init(void);
static void MX_USART2_UART_Init(void);
/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
void uart_clear_receive_buffer_and_start_receive_IT() 
{
    // Đọc dữ liệu trong bộ nhận cho đến khi không còn dữ liệu trong buffer
    while (__HAL_UART_GET_FLAG(&huart2, UART_FLAG_RXNE)) {
        char dummy_data = huart2.Instance->DR; // Đọc dữ liệu từ thanh ghi RDR để xóa dữ liệu trong bộ nhận
    }
    cnc.data.index = 0;
    HAL_UART_Receive_IT(&huart2, &cnc.data.Receive, 1);
}

void HAL_GPIO_EXTI_Callback(uint16_t GPIO_Pin)
{
  UNUSED(GPIO_Pin);
	// X Home
	if( GPIO_Pin == cnc.x_axis.PIN_HOME)
	{
		cnc.x_axis.pwm = 0;
		PWM(&cnc.x_axis);
		cnc.x_axis.home = true;
	}
	// Y Home
	if( GPIO_Pin == cnc.y_axis.PIN_HOME)
	{
		cnc.y_axis.pwm = 0;
		PWM(&cnc.y_axis);
		cnc.y_axis.home = true;
	}
	// Z Home
	if( GPIO_Pin == cnc.z_axis.PIN_HOME)
	{
		cnc.z_axis.pwm = 0;
		PWM(&cnc.z_axis);
		cnc.z_axis.home = true;
	}
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
{
  UNUSED(huart);
	if(huart->Instance == huart2.Instance)
	{
		if((cnc.data.Receive != '!') && (cnc.data.Receive != 0)) //line feed Ascii
		{
			cnc.data.ReceiveBuff[cnc.data.index++] = cnc.data.Receive; //Save data in Rxbuff
		}
		else if (cnc.data.Receive == '!')
		{
			cnc.data.index = 0;
			//ProcessData(&cnc);	
      cnc.Mode = 6; //process data
      //return;
		}
		HAL_UART_Receive_IT(&huart2, &cnc.data.Receive, 1);
	}
}

void axisInit()
{
	cnc.x_axis.htim_motor = &htim3;
	cnc.y_axis.htim_motor = &htim3;
	cnc.z_axis.htim_motor = &htim3;
  cnc.drill.htim_motor = &htim3;
	
	// cnc.x_axis.htim_enc = &htim4;
	// cnc.y_axis.htim_enc = &htim2;
	// cnc.z_axis.htim_enc = &htim1;

  cnc.x_axis.enc = &htim4.Instance->CNT;
  cnc.y_axis.enc = &htim2.Instance->CNT;
  cnc.z_axis.enc = &htim1.Instance->CNT;
	
	cnc.x_axis.GPIO_DIR = GPIOB;
	cnc.y_axis.GPIO_DIR = GPIOB;
	cnc.z_axis.GPIO_DIR = GPIOA;
  cnc.drill.GPIO_DIR = GPIOA;
	
	cnc.x_axis.PIN_DIR = GPIO_PIN_8;
	cnc.y_axis.PIN_DIR = GPIO_PIN_9;
	cnc.z_axis.PIN_DIR = GPIO_PIN_5;
  cnc.drill.PIN_DIR = GPIO_PIN_4;
	
	cnc.x_axis.CHANNEL = TIM_CHANNEL_3;
	cnc.y_axis.CHANNEL = TIM_CHANNEL_4;
	cnc.z_axis.CHANNEL = TIM_CHANNEL_2;
  cnc.drill.CHANNEL = TIM_CHANNEL_1;
	
	cnc.x_axis.GPIO_HOME = GPIOA;
	cnc.y_axis.GPIO_HOME = GPIOA;
	cnc.z_axis.GPIO_HOME = GPIOA;
	
	cnc.x_axis.PIN_HOME = GPIO_PIN_12;
	cnc.y_axis.PIN_HOME = GPIO_PIN_11;
	cnc.z_axis.PIN_HOME = GPIO_PIN_10;
	
	cnc.x_axis.Kp = 20;
	cnc.y_axis.Kp = 32;
	cnc.z_axis.Kp = 1;
	
	cnc.x_axis.Ki = 0.0001;
	cnc.y_axis.Ki = 0.0001;
	cnc.z_axis.Ki = 0.0001;
	
	cnc.x_axis.Kd = 0.1;
	cnc.y_axis.Kd = 0.1;
	cnc.z_axis.Kd = 0.02;
	
	cnc.x_axis.mm_pulse = 249.8886;//142.8571;
	cnc.y_axis.mm_pulse = 495.1475;//333.3333;
	cnc.z_axis.mm_pulse = 500;
}
/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_TIM1_Init();
  MX_TIM2_Init();
  MX_TIM3_Init();
  MX_TIM4_Init();
  MX_USART2_UART_Init();
  /* USER CODE BEGIN 2 */
  HAL_UART_Receive_IT(&huart2, &cnc.data.Receive, 1);

  // Init axis
	axisInit();

  // Start PWM
	HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
  HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_2);
  HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_3);
  HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_4);
	
	// Start Encoder
	HAL_TIM_Encoder_Start(&htim1, TIM_CHANNEL_1 | TIM_CHANNEL_2);
	HAL_TIM_Encoder_Start(&htim2, TIM_CHANNEL_1 | TIM_CHANNEL_2);
	HAL_TIM_Encoder_Start(&htim4, TIM_CHANNEL_1 | TIM_CHANNEL_2);

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    switch (cnc.Mode)
    {
    case 1: // mode Home

      // goto x home
			HOME(&cnc.x_axis);

			// goto y home
			HOME(&cnc.y_axis);
			
			// goto z home
			HOME(&cnc.z_axis);

      //test drill
      //runDrill(&cnc.drill, 70);
			
			if(cnc.x_axis.home && cnc.y_axis.home && cnc.z_axis.home)
			{
			  // cnc.x_axis.htim_enc->Instance->CNT = 0;
				// cnc.y_axis.htim_enc->Instance->CNT = 0;
				// cnc.z_axis.htim_enc->Instance->CNT = 0;

        *cnc.x_axis.enc = 0;
        *cnc.y_axis.enc = 0;
        *cnc.z_axis.enc = 0;
				
				cnc.x_axis.pos = 0;
				cnc.y_axis.pos = 0;
				cnc.z_axis.pos = 0;
				
			  cnc.Mode = 2; // to protect switch, goto home 1cm mode					
			}
        
      break;

    case 2: // mode Home 1cm
      move(&cnc.x_axis, 10);
			move(&cnc.y_axis, 10);
			move(&cnc.z_axis, 10);
			if( cnc.x_axis.finish && cnc.y_axis.finish && cnc.z_axis.finish)
			{
				cnc.x_axis.finish = false;
				cnc.y_axis.finish = false;
				cnc.z_axis.finish = false;

				// cnc.x_axis.htim_enc->Instance->CNT = 0;
				// cnc.y_axis.htim_enc->Instance->CNT = 0;
				// cnc.z_axis.htim_enc->Instance->CNT = 0;

        *cnc.x_axis.enc = 0;
        *cnc.y_axis.enc = 0;
        *cnc.z_axis.enc = 0;
				
				cnc.x_axis.pos = 0;
				cnc.y_axis.pos = 0;
				cnc.z_axis.pos = 0;
        memset(cnc.data.TransBuff, 0, sizeof(cnc.data.TransBuff));
        sprintf(cnc.data.TransBuff, "H!");
        HAL_UART_Transmit(&huart2, cnc.data.TransBuff, 2, 100);
        cnc.data.index = 0;
				cnc.Mode = 0;
        //uart_clear_receive_buffer_and_start_receive_IT();
			}
      break;

    case 3: // check drill
      if(cnc.drill.status != cnc.drill.enb)
			{
				if(cnc.drill.enb){
          cnc.thickness = 1;//thickness;
          runDrill(&cnc.drill, 70);
        }					
				else
					cnc.thickness = 10;					
				while(!cnc.z_axis.finish)
				{
					move(&cnc.z_axis, Z_MAX - cnc.thickness);
				}
				cnc.z_axis.finish = false;
				cnc.drill.status = cnc.drill.enb;
			}
      if(cnc.drill.enb)
        cnc.Mode = 5; // mode G01
      else
      {
        cnc.Mode = 4; // mode G00
        runDrill(&cnc.drill, 0);
      }        
      break;

    case 4: // mode G00
      while(!(cnc.x_axis.finish && cnc.y_axis.finish))
			{
				move(&cnc.x_axis, cnc.x_axis.next);
				move(&cnc.y_axis, cnc.y_axis.next);	
			}
			cnc.x_axis.finish = false;
			cnc.y_axis.finish = false;
			// cnc.x_axis.last = cnc.x_axis.next;
			// cnc.y_axis.last = cnc.y_axis.next;
      cnc.x_axis.last = cnc.x_axis.pos / cnc.x_axis.mm_pulse;
      cnc.y_axis.last = cnc.y_axis.pos / cnc.y_axis.mm_pulse;
      cnc.Mode = 7; // mode send feedback
      break;

    case 5: // mode G01
      drawLine(&cnc.x_axis, &cnc.y_axis);
      cnc.Mode = 7; // mode send feedback
      break;

    case 6: // mode process data
      ProcessData(&cnc);
      break;

    case 7: // send feedback to main      
      memset(cnc.data.TransBuff, 0, sizeof(cnc.data.TransBuff));
      sprintf(cnc.data.TransBuff, "G0%uX%0.2fY%0.2f!", cnc.drill.enb, cnc.x_axis.last, cnc.y_axis.last);
      //uart_clear_receive_buffer_and_start_receive_IT();
      HAL_Delay(100);
      HAL_UART_Transmit(&huart2, cnc.data.TransBuff, sizeof(cnc.data.TransBuff), 100);
      cnc.Mode = 0;      
      break;

    default:
      break;
    }
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_ON;
  RCC_OscInitStruct.HSEPredivValue = RCC_HSE_PREDIV_DIV1;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL9;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_2) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief TIM1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM1_Init(void)
{

  /* USER CODE BEGIN TIM1_Init 0 */

  /* USER CODE END TIM1_Init 0 */

  TIM_Encoder_InitTypeDef sConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM1_Init 1 */

  /* USER CODE END TIM1_Init 1 */
  htim1.Instance = TIM1;
  htim1.Init.Prescaler = 0;
  htim1.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim1.Init.Period = 65535;
  htim1.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim1.Init.RepetitionCounter = 0;
  htim1.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  sConfig.EncoderMode = TIM_ENCODERMODE_TI12;
  sConfig.IC1Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC1Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC1Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC1Filter = 0;
  sConfig.IC2Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC2Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC2Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC2Filter = 0;
  if (HAL_TIM_Encoder_Init(&htim1, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim1, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM1_Init 2 */

  /* USER CODE END TIM1_Init 2 */

}

/**
  * @brief TIM2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM2_Init(void)
{

  /* USER CODE BEGIN TIM2_Init 0 */

  /* USER CODE END TIM2_Init 0 */

  TIM_Encoder_InitTypeDef sConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM2_Init 1 */

  /* USER CODE END TIM2_Init 1 */
  htim2.Instance = TIM2;
  htim2.Init.Prescaler = 0;
  htim2.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim2.Init.Period = 65535;
  htim2.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim2.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  sConfig.EncoderMode = TIM_ENCODERMODE_TI12;
  sConfig.IC1Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC1Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC1Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC1Filter = 0;
  sConfig.IC2Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC2Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC2Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC2Filter = 0;
  if (HAL_TIM_Encoder_Init(&htim2, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim2, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM2_Init 2 */

  /* USER CODE END TIM2_Init 2 */

}

/**
  * @brief TIM3 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM3_Init(void)
{

  /* USER CODE BEGIN TIM3_Init 0 */

  /* USER CODE END TIM3_Init 0 */

  TIM_ClockConfigTypeDef sClockSourceConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};
  TIM_OC_InitTypeDef sConfigOC = {0};

  /* USER CODE BEGIN TIM3_Init 1 */

  /* USER CODE END TIM3_Init 1 */
  htim3.Instance = TIM3;
  htim3.Init.Prescaler = 29;
  htim3.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim3.Init.Period = 99;
  htim3.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim3.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim3) != HAL_OK)
  {
    Error_Handler();
  }
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  if (HAL_TIM_ConfigClockSource(&htim3, &sClockSourceConfig) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim3) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim3, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_2) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_3) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_4) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM3_Init 2 */

  /* USER CODE END TIM3_Init 2 */
  HAL_TIM_MspPostInit(&htim3);

}

/**
  * @brief TIM4 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM4_Init(void)
{

  /* USER CODE BEGIN TIM4_Init 0 */

  /* USER CODE END TIM4_Init 0 */

  TIM_Encoder_InitTypeDef sConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM4_Init 1 */

  /* USER CODE END TIM4_Init 1 */
  htim4.Instance = TIM4;
  htim4.Init.Prescaler = 0;
  htim4.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim4.Init.Period = 65535;
  htim4.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim4.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  sConfig.EncoderMode = TIM_ENCODERMODE_TI12;
  sConfig.IC1Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC1Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC1Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC1Filter = 0;
  sConfig.IC2Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC2Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC2Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC2Filter = 0;
  if (HAL_TIM_Encoder_Init(&htim4, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim4, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM4_Init 2 */

  /* USER CODE END TIM4_Init 2 */

}

/**
  * @brief USART2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART2_UART_Init(void)
{

  /* USER CODE BEGIN USART2_Init 0 */

  /* USER CODE END USART2_Init 0 */

  /* USER CODE BEGIN USART2_Init 1 */

  /* USER CODE END USART2_Init 1 */
  huart2.Instance = USART2;
  huart2.Init.BaudRate = 115200;
  huart2.Init.WordLength = UART_WORDLENGTH_8B;
  huart2.Init.StopBits = UART_STOPBITS_1;
  huart2.Init.Parity = UART_PARITY_NONE;
  huart2.Init.Mode = UART_MODE_TX_RX;
  huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart2.Init.OverSampling = UART_OVERSAMPLING_16;
  if (HAL_UART_Init(&huart2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART2_Init 2 */

  /* USER CODE END USART2_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOD_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();
  __HAL_RCC_GPIOB_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOA, GPIO_PIN_4|GPIO_PIN_5, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOB, GPIO_PIN_8|GPIO_PIN_9, GPIO_PIN_RESET);

  /*Configure GPIO pins : PA4 PA5 */
  GPIO_InitStruct.Pin = GPIO_PIN_4|GPIO_PIN_5;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /*Configure GPIO pins : PA10 PA11 PA12 */
  GPIO_InitStruct.Pin = GPIO_PIN_10|GPIO_PIN_11|GPIO_PIN_12;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_FALLING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /*Configure GPIO pins : PB8 PB9 */
  GPIO_InitStruct.Pin = GPIO_PIN_8|GPIO_PIN_9;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /* EXTI interrupt init*/
  HAL_NVIC_SetPriority(EXTI15_10_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(EXTI15_10_IRQn);

}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

