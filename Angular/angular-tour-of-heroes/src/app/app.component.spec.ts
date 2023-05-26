import { TestBed, ComponentFixture } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { HeroesComponent } from './components/heroes/heroes.component';
import { MessagesComponent } from './components/messages/messages.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { HeroDetailComponent } from './components/hero-detail/hero-detail.component';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  const expectedTitle = 'Tour of Heroes';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        FormsModule,
        MatToolbarModule,
        MatCardModule,
        MatInputModule,
        BrowserAnimationsModule,
        MatFormFieldModule,
        MatSidenavModule,
        MatListModule,
        MatIconModule,
        MatTabsModule
      ],
      declarations: [
        AppComponent,
        HeroesComponent,
        HeroDetailComponent,
        MessagesComponent
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it(`should have as title '${expectedTitle}'`, () => {
    expect(component.title).toEqual(expectedTitle);
  });

  it('should render title', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('#title')?.textContent).toEqual(expectedTitle);
  });

  it('should render links', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    let links = Array.from(compiled.querySelectorAll('#nav-bar a'))
      .map(x => x.textContent)
    expect(links).toEqual(["Dashboard", "Heroes"]);
  });

});
