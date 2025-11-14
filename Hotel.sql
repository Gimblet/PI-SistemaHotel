create database SistemaHotel
go
use SistemaHotel
go
create table tipoDoc(
id int identity primary key,
tipo varchar(15)
);
go
create table cliente(
id int identity primary key,
nombre varchar(20),
apellido varchar(20),
tipoId int foreign key references tipoDoc (id),
numeroDoc varchar(15) unique,
telefono varchar(15)
);
go
create table tipoHab(
id int identity primary key,
tipo varchar(15),
descripcion varchar(250),
precio decimal(8,2)
);
go
create table estado(
id int identity primary key,
estado varchar(15)
);
go
create table habitacion(
id int identity primary key,
numero varchar(5),
tipoId int foreign key references tipoHab (id),
estadoId int foreign key references estado (id)
);
go
create table alquiler(
id int identity primary key,
idHab int foreign key references habitacion (id),
idCli int foreign key references cliente (id),
dias int,
fechaActual date,
fechaEntrada date,
fechaSalida date,
total decimal(8,2)
);
go

insert into estado(estado) values 
('Disponible'),
('Ocupado'),
('Limpieza'),
('Mantenimiento');
go
insert into tipoHab(tipo,descripcion,precio)
values
('Matrimonial', 'Experiencia de lujo ideal para parejas: habitación con elegante decoración, TV Smart UHD, acceso a Netflix y Cable Premium, aire acondicionado de última generación, sistema de sonido envolvente y baño privado con agua caliente 24/7.', 150),
('Doble', 'Habitación espaciosa para amigos o familia: incluye TV Smart UHD, acceso a Netflix y Cable Premium, aire acondicionado de alta tecnología, sistema de sonido envolvente, baño con agua caliente 24/7 y relajante jacuzzi.', 200),
('Triple', 'La máxima comodidad y lujo para grupos: decoración exclusiva, TV Smart UHD, acceso a Netflix y Cable Premium, aire acondicionado de última generación, sistema de sonido envolvente, baño con agua caliente 24/7 y un amplio jacuzzi para disfrutar.', 250);
go
insert into habitacion(numero,tipoId,estadoId)
values('101',1,1),
('102',2,1),
('103',1,3),
('104',3,3),
('201',1,4),
('202',3,1),
('203',1,1),
('204',2,1),
('301',1,2),
('302',2,1),
('303',3,1),
('304',1,1);
go
insert into tipoDoc (tipo)
values('DNI'),('Pasaporte'),('CI'),('RUP'),('CE');
go
select * from alquiler;
go
select * from cliente;
go
select * from habitacion;
go

select * from estado;
go
select * from tipoDoc;
go
select * from tipoHab;
go
--Login
create table acceso(
id int primary key identity,
idCli int foreign key references cliente(id),
usuario varchar(20) unique,
clave varchar(50),
tipoUsuario varchar(15)
);
go
select * from acceso;
go
insert into cliente (nombre,apellido,tipoId,numeroDoc,telefono) values
('Christhian','Peralta',5,'006025040','970673811');
go
insert into acceso(idCli,usuario,clave,tipoUsuario) values
(1,'chris','123','ADMIN');
go