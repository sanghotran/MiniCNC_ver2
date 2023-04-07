#include "MyMath.h"

int int_abs(int number)
{
	if (number < 0)
		number = -number;
	return number;
}

int int_min(int number1, int number2)
{
	if(number1 < number2)
		return number1;
	else
		return number2;
}

int int_max(int number1, int number2)
{
	if(number1 > number2)
		return number1;
	else
		return number2;
}
