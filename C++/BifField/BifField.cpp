#include <iostream>

using namespace std;
struct DATA
{
    unsigned int a : 8;
    unsigned int b : 8;
    unsigned int c : 8;
    unsigned int d : 8;
};
union u_data
{
    int d1;
    double d2;
    char d3;
};
union u_Data2
{
    DATA sdata;
    unsigned int data;
};
int main()
{
    std::cout << "구조체의 크기 = " << sizeof(DATA) << endl;
    u_data data;
    data.d1 = 100;
    data.d2 = 300.22;
    data.d3 = 'a';
    std::cout << "d1 크기 = " << sizeof(data.d1) << endl;
    std::cout << "d2 크기 = " << sizeof(data.d2) << endl;
    std::cout << "d3 크기 = " << sizeof(data.d3) << endl;
    std::cout << "d1 값 = " << data.d1 << endl;
    std::cout << "d2 값 = " << data.d2 << endl;
    std::cout << "d3 값 = " << data.d3 << endl;
    std::cout << "공용체의 크기 = " << sizeof(u_data) << endl;
    u_Data2 tmp;
    tmp.sdata.a = 100;
    tmp.sdata.b = 200;
    tmp.sdata.c = 210;
    tmp.sdata.d = 220;
    std::cout << "tmp.a 값 = " << tmp.sdata.a << endl;
    std::cout << "tmp.b 값 = " << tmp.sdata.b << endl;
    std::cout << "tmp.c 값 = " << tmp.sdata.c << endl;
    std::cout << "tmp.d 값 = " << tmp.sdata.d << endl;
    std::cout << "tmp.data 값 = " << tmp.data << endl;
    //tmp의 데이터 data를 tmp2에 대입
    u_Data2 tmp2;
    tmp2.data = tmp.data;
    std::cout << "tmp2.a 값 = " << tmp2.sdata.a << endl;
    std::cout << "tmp2.b 값 = " << tmp2.sdata.b << endl;
    std::cout << "tmp2.c 값 = " << tmp2.sdata.c << endl;
    std::cout << "tmp2.d 값 = " << tmp2.sdata.d << endl;
    std::cout << "tmp2.data 값 = " << tmp2.data << endl;
}


