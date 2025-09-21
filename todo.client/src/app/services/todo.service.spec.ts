import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TodoService } from './todo.service';
import { Todo, CreateTodoRequest } from '../models/todo.model';
import { environment } from 'src/environments/environment';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiBaseUrl}/api/todos`;

  const mockTodos: Todo[] = [
    {
      id: 1,
      title: 'Test Todo 1',
      isCompleted: false,
      createdAt: new Date().toISOString()
    },
    {
      id: 2,
      title: 'Test Todo 2',
      isCompleted: true,
      createdAt: new Date().toISOString()
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TodoService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('loadTodos', () => {
    it('should load todos and update the subject', () => {
      service.loadTodos().subscribe(todos => {
        expect(todos).toEqual(mockTodos);
        expect(service.getTodos()).toEqual(mockTodos);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockTodos);
    });
  });

  describe('createTodo', () => {
    it('should create a new todo and update local state', () => {
      const request: CreateTodoRequest = { title: 'New Todo' };
      const newTodo: Todo = {
        id: 3,
        title: 'New Todo',
        isCompleted: false,
        createdAt: new Date().toISOString()
      };

      // First load some initial todos
      service.loadTodos().subscribe();
      const loadReq = httpMock.expectOne(apiUrl);
      loadReq.flush(mockTodos);

      // Now create a new todo
      service.createTodo(request).subscribe(todo => {
        expect(todo).toEqual(newTodo);
        expect(service.getTodos()).toEqual([...mockTodos, newTodo]);
      });

      const createReq = httpMock.expectOne(apiUrl);
      expect(createReq.request.method).toBe('POST');
      expect(createReq.request.body).toEqual(request);
      createReq.flush(newTodo);
    });

    it('should add todo to empty list', () => {
      const request: CreateTodoRequest = { title: 'First Todo' };
      const newTodo: Todo = {
        id: 1,
        title: 'First Todo',
        isCompleted: false,
        createdAt: new Date().toISOString()
      };

      service.createTodo(request).subscribe(todo => {
        expect(todo).toEqual(newTodo);
        expect(service.getTodos()).toEqual([newTodo]);
      });

      const createReq = httpMock.expectOne(apiUrl);
      expect(createReq.request.method).toBe('POST');
      createReq.flush(newTodo);
    });
  });

  describe('deleteTodo', () => {
    it('should delete a todo and update local state', () => {
      const todoId = 1;

      // First load some initial todos
      service.loadTodos().subscribe();
      const loadReq = httpMock.expectOne(apiUrl);
      loadReq.flush(mockTodos);

      // Now delete a todo
      service.deleteTodo(todoId).subscribe(() => {
        const remainingTodos = service.getTodos();
        expect(remainingTodos).toEqual(mockTodos.filter(t => t.id !== todoId));
        expect(remainingTodos.length).toBe(1);
      });

      const deleteReq = httpMock.expectOne(`${apiUrl}/${todoId}`);
      expect(deleteReq.request.method).toBe('DELETE');
      deleteReq.flush(null);
    });

    it('should handle delete when todo does not exist locally', () => {
      const todoId = 999;

      // Load initial todos
      service.loadTodos().subscribe();
      const loadReq = httpMock.expectOne(apiUrl);
      loadReq.flush(mockTodos);

      const initialCount = service.getTodos().length;

      service.deleteTodo(todoId).subscribe(() => {
        // Local state should remain unchanged since todo wasn't found
        expect(service.getTodos().length).toBe(initialCount);
      });

      const deleteReq = httpMock.expectOne(`${apiUrl}/${todoId}`);
      expect(deleteReq.request.method).toBe('DELETE');
      deleteReq.flush(null);
    });
  });

  describe('getTodos', () => {
    it('should return current todos', () => {
      // Initially should be empty
      expect(service.getTodos()).toEqual([]);

      // After loading, should return loaded todos
      service.loadTodos().subscribe();
      const req = httpMock.expectOne(apiUrl);
      req.flush(mockTodos);

      expect(service.getTodos()).toEqual(mockTodos);
    });
  });

  describe('todos$ observable', () => {
    it('should emit updated todos', () => {
      let emittedTodos: Todo[] = [];
      
      service.todos$.subscribe(todos => {
        emittedTodos = todos;
      });

      service.loadTodos().subscribe();
      const req = httpMock.expectOne(apiUrl);
      req.flush(mockTodos);

      expect(emittedTodos).toEqual(mockTodos);
    });

    it('should emit updated todos after create', () => {
      const newTodo: Todo = {
        id: 3,
        title: 'New Todo',
        isCompleted: false,
        createdAt: new Date().toISOString()
      };

      let emittedTodos: Todo[] = [];
      service.todos$.subscribe(todos => {
        emittedTodos = todos;
      });

      // Load initial todos
      service.loadTodos().subscribe();
      const loadReq = httpMock.expectOne(apiUrl);
      loadReq.flush(mockTodos);

      // Create new todo
      service.createTodo({ title: 'New Todo' }).subscribe();
      const createReq = httpMock.expectOne(apiUrl);
      createReq.flush(newTodo);

      expect(emittedTodos).toEqual([...mockTodos, newTodo]);
    });

    it('should emit updated todos after delete', () => {
      let emittedTodos: Todo[] = [];
      service.todos$.subscribe(todos => {
        emittedTodos = todos;
      });

      // Load initial todos
      service.loadTodos().subscribe();
      const loadReq = httpMock.expectOne(apiUrl);
      loadReq.flush(mockTodos);

      // Delete todo
      service.deleteTodo(1).subscribe();
      const deleteReq = httpMock.expectOne(`${apiUrl}/1`);
      deleteReq.flush(null);

      expect(emittedTodos).toEqual(mockTodos.filter(t => t.id !== 1));
    });
  });
});