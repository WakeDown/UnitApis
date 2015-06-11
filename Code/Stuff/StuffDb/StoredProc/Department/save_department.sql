CREATE PROCEDURE [dbo].[save_department]
    @id INT = NULL ,
    @name NVARCHAR(150) ,
    @id_parent INT = NULL ,
    @id_chief INT = NULL
AS
    BEGIN
	SET NOCOUNT ON;
        IF @id IS NOT NULL
            AND @id > 0
            AND EXISTS ( SELECT 1
                         FROM   departments d
                         WHERE  id = @id )
            BEGIN
                UPDATE  departments
                SET     name = @name ,
                        id_parent = @id_parent ,
                        id_chief = @id_chief
                WHERE   id = @id
            END
        ELSE
            BEGIN
                INSERT  INTO departments
                        ( name ,
                          id_parent ,
                          id_chief 
                        )
                VALUES  ( @name ,
                          @id_parent ,
                          @id_chief 
                        )
            END
	 

    END