CREATE PROCEDURE [dbo].[get_department] @id INT = NULL
AS
    BEGIN
        SET NOCOUNT ON;
        SELECT  id, name, id_parent, id_chief
        FROM    departments d
        WHERE   d.ENABLED = 1
                AND ( @id IS NULL
                      OR ( @id IS NOT NULL
                           AND @id > 0
                           AND d.id = @id
                         )
                    )
    END