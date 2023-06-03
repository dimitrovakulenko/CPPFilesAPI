// a comment
/////////////////////////////////////////////////////////////////////////

#include "some.h"
#include "some2.h"

namespace
{
std::pair<int, OdString> function(){
    auto n = 5;
    int k = 8;
    auto n2 = n;
    return {};
}

class MyClass {
public:
    int getFunctionGood() const {
        return 42;
    }
    int getFunctionBad() {
        return 42;
    }

};

}