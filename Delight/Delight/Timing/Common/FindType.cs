﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing.Common
{
    public enum FindType
    {
        /// <summary>
        /// 오직 시작 지점을 찾습니다.
        /// </summary>
        FindStartPoint,
        /// <summary>
        /// 오직 종료 지점을 찾습니다.
        /// </summary>
        FindEndPoint,
        /// <summary>
        /// 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindContains,
    }
}
