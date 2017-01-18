package models

// Song ..
type Song struct {
	Audit

	Index     uint16 `gorm:"not null"                    json:"index"`
	Title     string `gorm:"not null"                    json:"title"`
	TitleSlug string `gorm:"not null;unique_index"       json:"title_slug"`
	Length    uint32 `gorm:"not null"                    json:"length"`
	IsSingle  bool   `gorm:"not null"                    json:"is_single"`
	AlbumID   string `gorm:"not null"                    json:"album_id"`
	Album     Album  `gorm:"not null;ForeignKey:AlbumID" json:"album"`

	Producers []Person `gorm:"not null;many2many:song_producers;" json:"producers"`
	Genres    []Genre  `gorm:"not null;many2many:song_genres;"    json:"genres"`
	Writers   []Person `gorm:"not null;many2many:song_writers;"   json:"writers"`
	Featuring []Person `gorm:"not null;many2many:song_featuring;" json:"featuring"`
	Lyrics    []Lyric  `gorm:"not null;ForeignKey:SongID"         json:"lyrics"`

	ProducerIDs  []string `gorm:"-" json:"producer_ids"`
	GenreIDs     []string `gorm:"-" json:"genre_ids"`
	WriterIDs    []string `gorm:"-" json:"writer_ids"`
	FeaturingIDs []string `gorm:"-" json:"featuring_ids"`
}

// SongResponse ..
type SongResponse struct {
	Audit

	Index     uint16         `json:"index"`
	Title     string         `json:"title"`
	TitleSlug string         `json:"title_slug"`
	Length    uint32         `json:"length"`
	IsSingle  bool           `json:"is_single"`
	Album     *AlbumResponse `json:"album,omitempty"`

	Producers []*PersonResponse `json:"producers"`
	Genres    []*GenreResponse  `json:"genres"`
	Writers   []*PersonResponse `json:"writers"`
	Lyrics    []*LyricResponse  `json:"lyrics"`
	Featuring []*PersonResponse `json:"featuring"`
}

// Map ..
func (song Song) Map() *SongResponse {
	if song.ID == "" {
		return nil
	}

	sng := SongResponse{
		Audit: song.Audit,

		Index:     song.Index,
		Title:     song.Title,
		TitleSlug: song.TitleSlug,
		Length:    song.Length,
		IsSingle:  song.IsSingle,

		Album:     song.Album.Map(),
		Producers: make([]*PersonResponse, len(song.Producers)),
		Genres:    make([]*GenreResponse, len(song.Genres)),
		Writers:   make([]*PersonResponse, len(song.Writers)),
		Lyrics:    make([]*LyricResponse, len(song.Lyrics)),
		Featuring: make([]*PersonResponse, len(song.Featuring)),
	}

	for i, producer := range song.Producers {
		sng.Producers[i] = producer.Map()
	}
	for i, genre := range song.Genres {
		sng.Genres[i] = genre.Map()
	}
	for i, writer := range song.Writers {
		sng.Writers[i] = writer.Map()
	}
	for i, lyrics := range song.Lyrics {
		sng.Lyrics[i] = lyrics.Map()
	}
	for i, featuring := range song.Featuring {
		sng.Featuring[i] = featuring.Map()
	}

	return &sng
}
