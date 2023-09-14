#include <iostream>
using namespace std;
extern "C" int Fn();

int main()
{
	cout << "The result is: " << Fn() << endl;
	return 0;
}