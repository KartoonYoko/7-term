package main

import "fmt"

func fib(num int32) int32 {
	if num == 0 {
		return 0
	}
	if num == 1 {
		return 1
	}
	return fib(num-1) + fib(num-2)
}
func main() {
	var count int32
	for count = 0; count < 13; count++ {
		fmt.Print(fib(count))
		fmt.Print(" ")
	}
	var i int32
	for i = 0; i < 13; i++ {
		fmt.Print()
		fmt.Print("s")
	}
}
