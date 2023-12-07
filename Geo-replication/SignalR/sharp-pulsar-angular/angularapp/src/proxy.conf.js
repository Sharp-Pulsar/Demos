const PROXY_CONFIG = [
  {
    context: [
      "/chathub"
    ],

    target: "https://localhost:32772/",
    secure: false,
    ws: true,
    changeOrigin: false,
    logLevel: "debug"
  }
]

module.exports = PROXY_CONFIG;
