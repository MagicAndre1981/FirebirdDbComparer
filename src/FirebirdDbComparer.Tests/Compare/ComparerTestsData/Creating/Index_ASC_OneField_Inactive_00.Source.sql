﻿create table t (i int);

create asc index idx
on t(i);
alter index idx inactive;