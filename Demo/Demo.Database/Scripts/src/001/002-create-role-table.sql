create table auth.role
(
    id   smallserial,
    name varchar(50) not null,
    constraint pk_role primary key (id)
);

create unique index ux_role_name on auth.role (name);
