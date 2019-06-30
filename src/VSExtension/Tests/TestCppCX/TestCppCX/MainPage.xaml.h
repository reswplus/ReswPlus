//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"
#include "Strings/en/Resources.generated.h"

namespace TestCppCX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public ref class MainPage sealed
    {
    public:
        MainPage();
        property Object^ TestObject
        {
            Object^ get() {
                return ref new Windows::UI::Xaml::Controls::Button();
            }
        }

    };
}
