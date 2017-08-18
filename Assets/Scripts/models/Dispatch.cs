using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Dispatch
{
    void onFinish(string data);
    void onError(string error);
}
