﻿create table t (i int);

create desc index idx
on t(i);
alter index idx inactive;