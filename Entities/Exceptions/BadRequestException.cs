﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public abstract partial class BadRequestException:Exception  //abstract class cannot be new
    {
        protected BadRequestException(string message):base(message) //so that intead of public use protected keyword
        {
                
        }
            

    }
}
