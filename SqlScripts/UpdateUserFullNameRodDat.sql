SELECT *
--UPDATE e
--set e.full_name_rod = i.full_name_rod
FROM dbo.employees e 
INNER JOIN dbo.employee_import i ON e.id=i.id
WHERE e.enabled=1 and e.full_name_dat != i.full_name_dat

--UPDATE dbo.employee_import
--SET full_name_dat='Рябовой Ларисы Александровны'
--WHERE id=29

--SELECT * 
--FROM 
--update dbo.employee_import
--SET full_name_rod = N'Рябовой Ларисы Александровны', full_name_dat=N'Рябовой Ларисе Александровне'
--WHERE id=29

------SELECT * FROM 
--------update 
------dbo.employee_import
--------SET full_name_dat = full_name_rod, full_name_rod=full_name_dat
------WHERE id IN (
------177,
------254,
------257,
------261,
------263,
------265,
------270,
------271,
------272,
------273,
------274,
------275,
------276,
------277,
------278,
------279,
------281,
------282,
------284,
------285,
------286,
------289,
------290
------)