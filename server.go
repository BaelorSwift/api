package main

import (
	"log"

	c "github.com/baelorswift/api/controllers"
	middleware "github.com/baelorswift/api/middleware"
	m "github.com/baelorswift/api/models"
	s "github.com/baelorswift/api/services"
	raven "github.com/getsentry/raven-go"
	"github.com/gin-contrib/sentry"
	"github.com/jinzhu/configor"

	"gopkg.in/gin-gonic/gin.v1"
)

// Config contains the loaded configuration
var Config = struct {
	Address          string `json:"address"`
	Dsn              string `json:"dsn"`
	ConnectionString string `json:"connectionString"`
}{}

func main() {
	configor.Load(&Config, "config/app.json")
	raven.SetDSN(Config.Dsn)

	context := m.Context{Db: s.NewDatabase(Config.ConnectionString)}

	r := gin.New()
	r.Use(gin.Logger(), gin.Recovery())
	r.Use(middleware.JSONOnly())
	r.Use(sentry.Recovery(raven.DefaultClient, false))
	r.Static("/static", "./static/")
	v1 := r.Group("v1")
	{
		c.NewAlbumsController(v1, &context)
		c.NewGenresController(v1, &context)
		c.NewPeopleController(v1, &context)
		c.NewLabelsController(v1, &context)
	}

	log.Fatal(r.Run(Config.Address))
}
