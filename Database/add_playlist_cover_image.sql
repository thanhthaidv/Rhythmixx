IF COL_LENGTH('Playlists', 'CoverImageUrl') IS NULL
BEGIN
    ALTER TABLE Playlists
    ADD CoverImageUrl NVARCHAR(500) NULL;
END
