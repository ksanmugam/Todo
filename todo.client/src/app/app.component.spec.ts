import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { TodoService } from './services/todo.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { CreateTodoRequest, Todo } from './models/todo.model';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let todoServiceSpy: jasmine.SpyObj<TodoService>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;

  const mockTodos: Todo[] = [
    { id: 1, title: 'Test 1', isCompleted: false, createdAt: new Date().toISOString() },
    { id: 2, title: 'Test 2', isCompleted: true, createdAt: new Date().toISOString() }
  ];

  beforeEach(waitForAsync(() => {
    todoServiceSpy = jasmine.createSpyObj('TodoService', ['loadTodos', 'createTodo', 'deleteTodo'], {
      todos$: of(mockTodos)
    });

    snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);

    // Ensure loadTodos returns a valid observable BEFORE detectChanges
    todoServiceSpy.loadTodos.and.returnValue(of(mockTodos));

    TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        { provide: TodoService, useValue: todoServiceSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
        { provide: MatDialog, useValue: dialogSpy }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty title', () => {
    expect(component.todoForm.get('title')?.value).toBe('');
  });

  it('should load todos on init', () => {
    expect(todoServiceSpy.loadTodos).toHaveBeenCalled();
  });

  describe('onSubmit', () => {
    it('should not create todo when form is invalid', () => {
      component.todoForm.patchValue({ title: '' });
      component.onSubmit();
      expect(todoServiceSpy.createTodo).not.toHaveBeenCalled();
    });
  });

  it('should track todo by id', () => {
    const result = component.trackByTodoId(0, mockTodos[0]);
    expect(result).toBe(mockTodos[0].id);
  });
});