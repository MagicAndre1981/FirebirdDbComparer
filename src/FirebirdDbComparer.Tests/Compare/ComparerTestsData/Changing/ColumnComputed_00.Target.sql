﻿create table t (a char(1), b computed by (trim(a || a)));