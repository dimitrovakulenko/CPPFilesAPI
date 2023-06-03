// TestHeader.h
#pragma once

class TestClass1 {
public:
    // Maps to Constructor
    TestClass1();

    // Maps to Enum
    enum TestEnum1 {
        ENUM_VALUE1,
        ENUM_VALUE2
    };

    // Maps to StaticMethod
    static void staticMethod();

    // Maps to NonStaticMethod
    void nonStaticMethod();

    // Maps to CreationMethod
    static TestClass1* create();

private:
    // Maps to DataMember
    int dataMember1;

    // Maps to Constant
    static const int CONSTANT1 = 1;

protected:
    // Maps to Typedef
    typedef int Integer;

    // Maps to Destructor
    ~TestClass1();
};

class TestClass2 {
protected:
    // Maps to Typedef
    typedef double Real;

    // Maps to Constant
    static const double CONSTANT2 = 2.0;

public:
    // Maps to Constructor
    TestClass2();

    // Maps to Enum
    enum TestEnum2 {
        ENUM_VALUE3,
        ENUM_VALUE4
    };

protected:
    // Maps to StaticMethod
    static void staticMethod2();

    // Maps to NonStaticMethod
    void nonStaticMethod2();

    // Maps to CreationMethod
    static TestClass2* create();

private:
    // Maps to DataMember
    double dataMember2;

    // Maps to Destructor
    ~TestClass2();
};
