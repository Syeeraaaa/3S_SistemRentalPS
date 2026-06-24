create database SistemRental_PS;

create table Pelanggan (
	id_pelanggan int identity(1,1) primary key,
	nama_pelanggan varchar(100) not null,
	no_hp varchar(15),
	alamat varchar(200)
);

insert into Pelanggan (nama_pelanggan,no_hp) values
('Bambang','0812345678'),
('Tio','0899874567');

select*from Pelanggan;

ALTER TABLE Pelanggan
DROP COLUMN alamat;

create table UnitPS (
	id_unit int identity(1,1) primary key,
	nama_unit varchar(50) not null,
	tipe_ps varchar(20),
	harga_perjam int,
	status varchar(20) check (Status in ('Tersedia','Dipakai','Maintenance')) default 'Tersedia'
);

insert into UnitPS (nama_unit,tipe_ps,harga_perjam,status)
values 
('Unit-1','PS5','10000','Tersedia'),
('Unit-2','PS5','10000','Tersedia'),
('Unit-3','PS4','8000','Tersedia');

select*from UnitPS

create table Game(
	id_game int identity(1,1) primary key,
	id_unit int,
	nama_game varchar(100),
	genre varchar(50),

	foreign key (id_unit) references UnitPS(id_unit)
);

insert into Game (id_unit,nama_game,genre)
values 
(54,'FIFA 24','Sports'),
(55, 'GTA V', 'Action'),
(56, 'Tekken 8', 'Fighting'),
(57, 'FIFA 24', 'Sports'),
(58, 'Mortal Kombat', 'Fighting'),
(59, 'Naruto Storm', 'Fighting');

select*from Game;
select*from Laporan;

create table Transaksi (
	id_transaksi int identity(1,1) primary key,
	id_pelanggan int,
	id_unit int,
	tanggal date,
	jam_mulai time,
	jam_selesai time,
	total_bayar int,

	foreign key (id_pelanggan) references Pelanggan(id_pelanggan),
	foreign key (id_unit) references UnitPS(id_unit)
);

insert into Transaksi (id_pelanggan,id_unit,tanggal,jam_mulai,jam_selesai,total_bayar)
values 
(1, 1, '2026-03-06', '13:00', '15:00', 20000),
(2, 2, '2026-03-06', '14:00', '16:00', 20000);
select * from Transaksi;
SELECT * FROM UnitPS;

create table Admin (
	id_admin int identity(1,1) primary key,
	nama_admin varchar(150) not null,
	username varchar(100) not null,
	password varchar(100) not null);

insert into Admin (nama_admin,username,password) values 
('Shafa','Admin','admin123');

select * from Admin

--Laporan
SELECT 
p.nama_pelanggan,
u.nama_unit,
t.tanggal,
t.jam_mulai,
t.jam_selesai,
t.total_bayar
FROM Transaksi t
JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
JOIN UnitPS u ON t.id_unit = u.id_unit;

--Buat view Laporan
create view vmLaporan as
select
	p.nama_pelanggan,
	u.nama_unit,
	u.tipe_ps,
	t.tanggal,
	t.jam_mulai,
	t.jam_selesai,
	t.total_bayar
from Transaksi t
join Pelanggan p on t.id_pelanggan = p.id_pelanggan
join UnitPS u on t.id_unit=u.id_unit;

select *from vmLaporan;
select *from Transaksi;
--stored procedure buat total pendapatan


create procedure sp_TotalPendapatan
	@Total int output
as
begin
	set nocount on;
	select @Total = sum(total_bayar) from Transaksi
end

ALTER procedure  sp_TotalPendapatan
	@tglMulai DATE,
	@tglSampai DATE,
	@Total INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT @Total = ISNULL(SUM(total_bayar), 0)
	from Transaksi
	WHERE tanggal BETWEEN @tglMulai AND @tglSampai
END

--SHAFA PUNYA --
alter PROCEDURE sp_InsertTransaksi
	@nama varchar(100), -- menambahkan nama sama no hp biar bias nambah nama sama no hp
	@nohp varchar(15),
    @id_unit INT,
    @jam_mulai DATETIME,
    @jam_selesai DATETIME,
    @total_bayar INT
AS
BEGIN
	set nocount on;
	declare @id_pelanggan int;
	insert into Pelanggan (nama_pelanggan, no_hp)
	values (@nama, @nohp);

	set @id_pelanggan = SCOPE_IDENTITY(); --buat ngambil nilai ID primary key

    INSERT INTO Transaksi (id_pelanggan, id_unit, tanggal, jam_mulai, jam_selesai, total_bayar)
    VALUES (@id_pelanggan, @id_unit, CAST(@jam_mulai AS DATE), @jam_mulai, @jam_selesai, @total_bayar);

	update UnitPS
	set status = 'Dipakai'
	where id_unit = @id_unit;
END
GO


CREATE PROCEDURE sp_UpdateTransaksi
    @id_transaksi INT,
    @id_unit INT,
    @jam_mulai DATETIME,
    @jam_selesai DATETIME,
    @total_bayar INT
AS
BEGIN
    UPDATE Transaksi
    SET id_unit = @id_unit,
        jam_mulai = @jam_mulai,
        jam_selesai = @jam_selesai,
        total_bayar = @total_bayar
    WHERE id_transaksi = @id_transaksi
END
GO


CREATE PROCEDURE sp_DeleteTransaksi
    @id_transaksi INT
AS
BEGIN
    DELETE FROM Transaksi WHERE id_transaksi = @id_transaksi
END
GO


CREATE PROCEDURE sp_SearchTransaksi
    @nama NVARCHAR(100)
AS
BEGIN
    SELECT t.id_transaksi, p.nama_pelanggan, p.no_hp, u.nama_unit,
           t.jam_mulai, t.jam_selesai, t.total_bayar
    FROM Transaksi t
    JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
    JOIN UnitPS u ON t.id_unit = u.id_unit
    WHERE p.nama_pelanggan LIKE '%' + @nama + '%'
END
GO


ALTER VIEW vwTransaksi AS
SELECT 
    t.id_transaksi,
    p.nama_pelanggan AS 'Nama Pelanggan',
    p.no_hp AS 'No HP',
	u.tipe_ps as 'Tipe PS',
    u.nama_unit AS 'Unit',
    t.jam_mulai as 'Jam Mulai',
    t.jam_selesai as 'Jam Selesai',
    t.total_bayar as 'Total bayar'
FROM Transaksi t
JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
JOIN UnitPS u ON t.id_unit = u.id_unit;


SELECT * FROM Transaksi
SELECT * FROM Pelanggan
SELECT * FROM UnitPS

-- =========================
-- SEKAR PUNYAAAAAAAA
-- =========================
CREATE VIEW vwUnitPS AS
SELECT
	id_unit,
	nama_unit,
	tipe_ps,
	harga_perjam,
	status
FROM UnitPS;

SELECT * INTO UnitPS_Backup FROM UnitPS

SELECT * FROM Game;
SELECT * FROM UnitPS;


--====================
--UNIT
--====================
CREATE PROCEDURE sp_GetUnit
AS
BEGIN
	SET NOCOUNT ON;
	SELECT id_unit,
	nama_unit,
	tipe_ps,
	harga_perjam,
	status
FROM UnitPS
END
	
CREATE PROCEDURE sp_GetUnitById
	@id_unit INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT nama_unit, 
	tipe_ps, 
	harga_perjam, 
	status
	FROM UnitPS
	WHERE id_unit = @id_unit
END

CREATE PROCEDURE sp_InsertUnitPS
	@nama_unit VARCHAR(100),
	@tipe_ps VARCHAR(100),
	@harga_perjam INT,
	@status VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	insert into UnitPS (nama_unit,tipe_ps,harga_perjam,status)
	values (@nama_unit, @tipe_ps, @harga_perjam, @status)
END

CREATE PROCEDURE sp_UpdateUnitPS
	@id_unit INT,
	@nama_unit VARCHAR(100),
	@tipe_ps VARCHAR(100),
	@harga_perjam INT,
	@status VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE UnitPS
    SET nama_unit = @nama_unit,
        tipe_ps = @tipe_ps,
        harga_perjam = @harga_perjam,
        status = @status
     WHERE id_unit = @id_unit
END	

CREATE PROCEDURE sp_DeleteUnitPS
	@id_unit INT
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM UnitPS
	WHERE id_unit = @id_unit
END

-- ===============================
-- GAME
--================================
alter PROCEDURE sp_GetGamePS
AS
BEGIN
	SET NOCOUNT ON;
	SELECT id_game,
	id_unit,
	nama_game,
	genre
FROM Game
END

alter PROCEDURE sp_GetGameById
	@id_game INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT id_unit,
	nama_game,
	genre
	FROM Game
	WHERE id_game = @id_game
END
--===========================
CREATE PROCEDURE sp_InsertGamePS
	@id_unit int,
	@nama_game varchar(100),
	@genre varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Game
    (id_unit,nama_game,genre)
    VALUES
    (@id_unit,@nama_game,@genre)
END
--==============================
CREATE PROCEDURE sp_UpdateGamePS
	@id_game int,
	@id_unit int,
	@nama_game varchar(100),
	@genre varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE Game
    SET 
    id_unit = @id_unit,
    nama_game = @nama_game,
    genre = @genre
    WHERE id_game = @id_game
END	
--========================

CREATE PROCEDURE sp_DeleteGamePS
	@id_game INT
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM Game
	WHERE id_game = @id_game
END

--buat ngefilter 
create procedure sp_GetGameByFilter
	@tipe_ps varchar(100) = NULL,
	@id_unit int = Null
as
begin
	set nocount on;
	select
	g.id_game,
	u.tipe_ps as 'Tipe PS',
	u.id_unit,
	u.nama_unit as 'Nama Unit',
	g.nama_game as 'Nama Game',
	g.genre as 'Genre'
	from Game g
	left join UnitPS u On g.id_unit = u.id_unit
	where
		(@tipe_ps is NULL or u.tipe_ps = @tipe_ps)
		and (@id_unit is Null or g.id_unit = @id_unit)
	order by u.tipe_ps, u.nama_unit
end

--filter ini tuh buat ngefilter berdasarkan tipe PS
create procedure sp_GetUnitByTipe
	@tipe_ps varchar(100)
as
begin
	set nocount on;
	select id_unit, nama_unit
	from UnitPS
	where tipe_ps = @tipe_ps
end

exec sp_GetGameByFilter @id_unit = 58;
exec sp_GetGameByFilter @tipe_ps = 'PS4';
exec sp_GetGameByFilter @tipe_ps = 'PS4', @id_unit = 58;
select * from Game

create table LogError
(
	id_log int identity(1,1) primary key,
	waktu datetime,
	pesan_error varchar(max)
	);

--membuat LogAktivitas
create table LogAktivitas
(
	id_log int identity(1,1),
	aktivitas varchar(100),
	waktu datetime
);
-- TRIGGER INSERT

create trigger trg_InsertTransaksi
on Transaksi
after insert
as
begin
	insert into LogAktivitas
	values('Tambah Data Transaksi', getdate());
end;

select*from LogAktivitas

-- buat membuat trigger update kalo ada yang ngubah data di transaksi
create trigger trg_UpdateTransaksi
on Transaksi
after update
as
begin
	insert into LogAktivitas
	values('Update Data Transaksi', getdate());
end;

--BUAT KELOLA DATA_PS SAMA DATA GAME
create trigger trg_InsertDataPS
on UnitPS
after Insert
as
begin
	insert into LogAktivitas
	values('Tambah Data Unit PS', getdate());
end;

create trigger trg_InsertDataGame
on Game
after Insert
as
begin
	insert into LogAktivitas
	values('Tambah Data Game', getdate());
end;

-- buat semua aktivitas hapus data ps sama game 
create trigger trg_DeleteDataPs
on UnitPs
after delete
as
begin
	insert into LogAktivitas
	values('Hapus Data Unit PS', getdate());
end;

create trigger trg_DeleteGame
on Game
after delete
as
begin
	insert into LogAktivitas
	values('Hapus Data Game', getdate());
end;

--buat semua update data Ps sama Game
create trigger trg_UpdateDataPS --dihapus dulu lah 
on UnitPs
after update
as
begin
	insert into LogAktivitas
	values('Update Data Unit PS', getdate());
end;

-- UPDATE GAME
alter trigger trg_UpdateDataGame
on Game
after update
as
begin
	insert into LogAktivitas
	values('Update Data Game', getdate());
end;

--MEMBUAT TABEL SECURITY LOG
create Table LogKeamanan
(
	id_log int identity(1,1),
	aktivitas varchar(200),
	jumlah_data int,
	waktu datetime
);

CREATE TRIGGER trg_PreventMassUpdate
ON UnitPS
AFTER UPDATE
AS
BEGIN
	DECLARE @jumlah INT;
	SELECT @jumlah = COUNT(*) FROM inserted;

	-- Jika update lebih dari 5 data
	IF @jumlah > 5
	BEGIN
		--Simpan log keamanan
		INSERT INTO LogKeamanan
		VALUES(
			'Warning : Update massal terdeteksi',
			@jumlah,
			GETDATE()
		);

		--Membatalkan transaksi
		ROLLBACK TRANSACTION;
		
		RAISERROR(
		'Update dibatalkan! Terlalu banyak data diubah.',
		16,
		1
		);
	END
END;

select * from LogAktivitas

alter PROCEDURE sp_LaporanTransaksi
	@tglMulai DATE,
	@tglSampai DATE
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		p.nama_pelanggan AS 'NamaPelanggan',
		p.no_hp AS 'NoHP',
		u.tipe_ps AS 'Tipe PS',
		u.nama_unit AS 'Unit',
		t.tanggal as 'Tanggal',
		t.jam_mulai AS 'JamMulai',
		t.jam_selesai AS 'JamSelesai',
		t.total_bayar AS 'TotalBayar'
	FROM Transaksi t
	JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
	JOIN UnitPS u ON t.id_unit = u.id_unit
	WHERE t.tanggal >= @tglMulai
		AND t.tanggal <= @tglSampai
	ORDER BY t.tanggal DESC, t.jam_mulai DESC
END
GO

select * from Transaksi

alter PROCEDURE sp_UpdateUnitPS
	@id_unit INT,
	@nama_unit VARCHAR(100),
	@tipe_ps VARCHAR(100),
	@harga_perjam INT,
	@status VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	IF @nama_unit NOT LIKE 'Unit-%'
       OR ISNUMERIC(REPLACE(@nama_unit, 'Unit-', '')) = 0
    BEGIN
        RAISERROR('Format nama unit harus Unit-angka', 16, 1);
        RETURN;
    END

	IF @tipe_ps NOT LIKE 'PS%'
       OR ISNUMERIC(REPLACE(@tipe_ps, 'PS', '')) = 0
    BEGIN
        RAISERROR('Format tipe PS harus PS diikuti angka', 16, 1);
        RETURN;
    END

	IF ISNUMERIC(@harga_perjam) = 0 OR CAST(@harga_perjam AS INT) <= 0
    BEGIN
        RAISERROR('Harga per jam harus berupa angka positif tanpa simbol', 16, 1);
        RETURN;
    END

	UPDATE UnitPS
    SET nama_unit = @nama_unit,
        tipe_ps = @tipe_ps,
        harga_perjam = @harga_perjam,
        status = @status
     WHERE id_unit = @id_unit
END

alter PROCEDURE sp_InsertUnitPS
	@nama_unit VARCHAR(100),
	@tipe_ps VARCHAR(100),
	@harga_perjam INT,
	@status VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	IF @nama_unit NOT LIKE 'Unit-%'
		OR ISNUMERIC(REPLACE(@nama_unit, 'Unit-', '')) = 0
	BEGIN
		RAISERROR('Format nama unit harus diisi Unit-angka', 16,1);
		RETURN;
	END
	if exists (select 1 from UnitPS where nama_unit = @nama_unit)
	begin
		Raiserror('Nama Unit "%s" sudah terdaftar!', 16, 1, @nama_unit);
		return;
	end

	insert into UnitPS (nama_unit,tipe_ps,harga_perjam,status)
	values (@nama_unit, @tipe_ps, @harga_perjam, @status)
END

select * from LogAktivitas;
select * from LogError

select*from vwTransaksi
