#include <iostream>



int fib(int num) {
	if (num == 0) { return 0; }
	if (num == 1) { return 1; }
	return fib(num - 1) + fib(num - 2);
}

void main()
{
	int count;
	for (count = 0; count < 13; count++) {
		std::cout << fib(count) << " ";
	}
	int i;
	for (i = 0; i < 13; i++) {
		std::cout << i << "s";
	}
}



