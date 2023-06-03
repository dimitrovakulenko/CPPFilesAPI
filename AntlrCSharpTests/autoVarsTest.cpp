// a comment
/////////////////////////////////////////////////////////////////////////

#include "some.h"
#include "some2.h"

namespace
{
std::pair<int, OdString> function(){
    auto i1 = f(), i2 = 3, i3 = i1, i4;

    int usage1 = i2;
    int usage2 = 3 * i2;
    i2 = 5;

    int p = some2(3, "aaa", i4);

    return {};
}
}