const { createProxyMiddleware } = require('http-proxy-middleware');
module.exports = function(app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'http://localhost:5043/',
      changeOrigin: true,
      pathRewrite: {'^/api' : ''}
    })
  );
};
