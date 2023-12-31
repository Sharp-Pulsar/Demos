import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

export class SharpInterceptor implements HttpInterceptor
{
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!req.headers.has('Content-Type'))
    {
      req = req.clone({
        headers: req.headers.set('Content-Type', 'application/json')
      });
    }
    req = req.clone({ headers: req.headers.set('Accept', 'application/json') });
    return next.handle(req);
  }
}
