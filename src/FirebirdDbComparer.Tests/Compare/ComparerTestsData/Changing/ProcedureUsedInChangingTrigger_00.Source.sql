set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
end^

set term ;^

create table t (i int);

set term ^;

create trigger trig for t
before insert
as
begin
    execute procedure p(new.i) returning_values new.i;
    new.i = new.i * 2;
end^

set term ;^