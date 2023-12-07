import { TestBed } from '@angular/core/testing';
import { HomeComponent } from './../home/home.component';

describe('HomeComponent', () => {
  beforeEach(() => TestBed.configureTestingModule({
    declarations: [HomeComponent]
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(HomeComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'home'`, () => {
    const fixture = TestBed.createComponent(HomeComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('home');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.content span')?.textContent).toContain('home app is running!');
  });
});
