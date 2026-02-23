import axios, { AxiosError, AxiosInstance, AxiosRequestConfig } from "axios";

export type SemesterType = "Semester1" | "Semester2";
export type ExamType =
  | "TestWhenClassBegins"
  | "FifteenMinutesTest"
  | "FortyFiveMinutesTest"
  | "SemesterTest";

export interface RegisterUserPayload {
  userName: string;
  password: string;
  email: string;
}

export interface LoginPayload {
  userName: string;
  password: string;
  rememberMe?: boolean;
}

export interface RefreshTokenPayload {
  token: string;
}

export interface StudentPayload {
  fullName: string;
  address: string;
  className: string;
  academicYear: string;
  parentName: string;
  parentEmail: string;
}

export interface TeacherPayload {
  name: string;
  email: string;
  subjectId?: number;
  subjectName?: string;
}

export interface TeacherClassAssignPayload {
  teacherFullName: string;
  teacherEmail: string;
  className: string;
  subjectName: string;
}

export interface UpdateTeacherAssignPayload {
  teacherFullName: string;
  teacherEmail: string;
  currentClassName: string;
  newClassName: string;
}

export interface SemesterPayload {
  semesterId?: number;
  semesterType: SemesterType | string;
  startDate: string;
  endDate: string;
  academicYear: string;
}

export interface ClassPayload {
  className: string;
}

export interface UpdateClassAndResetPayload {
  currentAcademicYear: string;
  currentClassName: string;
  newAcademicYear: string;
  newClassName: string;
}

export interface CreateFeeNotificationPayload {
  semesterType: SemesterType | string;
  academicYear: string;
  amount: number;
  content: string;
}

export interface UpdateFeeNotificationPayload {
  semesterType: SemesterType;
  academicYear: string;
  amount: number;
  content: string;
}

export interface ScorePayload {
  studentId: number;
  subjectId: number;
  semesterId: number;
  examType: ExamType | number;
  scoreValue: number;
}

export interface PaymentRequestPayload {
  orderId?: string;
  amount?: number;
  createdDate?: string;
  notificationContent: string;
  semesterName: SemesterType | string;
  academicYear: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: string;
  role: string;
}

export interface TokenPair {
  accessToken: string;
  refreshToken?: string;
}

export interface TokenStorage {
  getAccessToken: () => string | null;
  getRefreshToken: () => string | null;
  setTokens: (tokens: TokenPair) => void;
  clear: () => void;
}

export const localStorageTokenStorage: TokenStorage = {
  getAccessToken: () =>
    typeof localStorage !== "undefined"
      ? localStorage.getItem("accessToken")
      : null,
  getRefreshToken: () =>
    typeof localStorage !== "undefined"
      ? localStorage.getItem("refreshToken")
      : null,
  setTokens: ({ accessToken, refreshToken }) => {
    if (typeof localStorage === "undefined") return;
    localStorage.setItem("accessToken", accessToken);
    if (refreshToken) localStorage.setItem("refreshToken", refreshToken);
  },
  clear: () => {
    if (typeof localStorage === "undefined") return;
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
  },
};

type RetryableConfig = AxiosRequestConfig & { _retry?: boolean };

export interface ApiClientOptions {
  baseURL?: string;
  tokenStorage?: TokenStorage;
}

export const createApiClient = (options: ApiClientOptions = {}) => {
  const baseURL = options.baseURL ?? "http://localhost:5000";
  const tokenStorage = options.tokenStorage ?? localStorageTokenStorage;

  const http = axios.create({
    baseURL,
    timeout: 30000,
  });

  attachAuthInterceptors(http, tokenStorage);

  return {
    http,

    auth: {
      register: (role: string, body: RegisterUserPayload) =>
        http.post("/api/Authenticate/Register", body, { params: { role } }),

      confirmEmail: (token: string, email: string) =>
        http.get("/api/Authenticate/ConfirmEmail", { params: { token, email } }),

      login: async (body: LoginPayload) => {
        const res = await http.post<LoginResponse>("/api/Authenticate/Login", body);
        tokenStorage.setTokens({
          accessToken: res.data.accessToken,
          refreshToken: res.data.refreshToken,
        });
        return res;
      },

      refreshToken: (token: string) =>
        http.post<{ accessToken: string }>("/api/Authenticate/refresh-token", { token }),
    },

    admin: {
      addStudent: (body: StudentPayload) => http.post("/api/Admin/AddStudent", body),
      deleteStudent: (studentId: number) =>
        http.delete(`/api/Admin/DeleteStudent/${studentId}`),
      updateStudent: (studentId: number, body: StudentPayload) =>
        http.put(`/api/Admin/UpdateStudent/${studentId}`, body),
      getAllStudents: () => http.get("/api/Admin/getaAllStudents"),

      addTeacher: (body: TeacherPayload) => http.post("/api/Admin/AddTeacher", body),
      deleteTeacher: (teacherId: number) =>
        http.delete(`/api/Admin/DeleteTeacher/${teacherId}`),
      updateTeacher: (teacherId: number, body: TeacherPayload) =>
        http.put(`/api/Admin/UpdateTeacher/${teacherId}`, body),
      getAllTeachers: () => http.get("/api/Admin/GetAllTeachers"),

      assignTeacherToClass: (body: TeacherClassAssignPayload) =>
        http.post("/api/Admin/AssignTeacherToClass", body),
      getTeacherClassAssigned: () => http.get("/api/Admin/GetTeacherClassAssigned"),
      updateTeacherClassAssignment: (body: UpdateTeacherAssignPayload) =>
        http.put("/api/Admin/UpdateTeacherClassAssignment", body),
      deleteTeacherFromClass: (body: TeacherClassAssignPayload) =>
        http.delete("/api/Admin/DeleteTeacherFromClass", { data: body }),

      addSemester: (body: SemesterPayload) => http.post("/api/Admin/AddSemester", body),
      updateSemester: (semesterId: number, body: SemesterPayload) =>
        http.put(`/api/Admin/UpdateSemester/${semesterId}`, body),
      deleteSemester: (semesterId: number) =>
        http.delete(`/api/Admin/DeleteSemesters/${semesterId}`),
      getAllSemesters: () => http.get("/api/Admin/GetAllSemesters"),

      addClass: (body: ClassPayload) => http.post("/api/Admin/AddClass", body),
      getAllClasses: () => http.get("/api/Admin/GetAllClasses"),
      getClassById: (id: number) => http.get(`/api/Admin/GetClass/${id}`),
      updateClass: (id: number, body: ClassPayload) =>
        http.put(`/api/Admin/UpdateClass/${id}`, body),
      deleteClass: (id: number) => http.delete(`/api/Admin/DeleteClass/${id}`),

      upgradeClass: (params: {
        oldClassId: number;
        oldAcademicYear: string;
        newClassId: number;
        newAcademicYear: string;
      }) => http.post("/api/Admin/UpgradeClass", null, { params }),

      calculateClassAverage: (className: string, academicYear: string) =>
        http.post("/api/Admin/calculate-class-average", null, {
          params: { className, academicYear },
        }),

      getAverageScores: (classId: number, academicYear: string) =>
        http.get("/api/Admin/getAverage-scores", { params: { classId, academicYear } }),

      updateClassAndResetScores: (body: UpdateClassAndResetPayload) =>
        http.post("/api/Admin/UpdateClassAndResetScores", body),

      createFeeNotification: (body: CreateFeeNotificationPayload) =>
        http.post("/api/Admin/CreateFeeNotification", body),
      updateFeeNotification: (body: UpdateFeeNotificationPayload) =>
        http.put("/api/Admin/UpdateFeeNotification", body),
      getTuitionFeeNotification: (semesterType: SemesterType, academicYear: string) =>
        http.get("/api/Admin/GetTuitionFeeNotification", {
          params: { semesterType, academicYear },
        }),
    },

    teacher: {
      viewAllSemesters: () => http.get("/api/Teacher/ViewAllSemesters"),
      getStudentsInAssignedClasses: () =>
        http.get("/api/Teacher/GetStudentsInAssignedClasses"),
      addScore: (body: ScorePayload) => http.post("/api/Teacher/AddScore", body),
      getScoreStudent: (studentId: number, subjectId?: number, semesterId?: number) =>
        http.get(`/api/Teacher/GetScoreStudent/${studentId}`, {
          params: { subjectId, semesterId },
        }),
      calculateSemesterAverage: (studentId: number, semesterId: number) =>
        http.get("/api/Teacher/CalculateSemesterAverage", {
          params: { studentId, semesterId },
        }),
      getSemesterAverage: (studentId: number, semesterId: number) =>
        http.get(`/api/Teacher/GetSemesterAverage/${studentId}/semester/${semesterId}`),
    },

    student: {
      getDailyScores: (academicYear: string) =>
        http.get("/api/Student/GetDailyScores", { params: { academicYear } }),
      getSubjectsAverageScores: (academicYear: string) =>
        http.get("/api/Student/GetSubjectsAverageScores", { params: { academicYear } }),
      getAverageScores: (academicYear: string) =>
        http.get("/api/Student/GetAverageScores", { params: { academicYear } }),
    },

    parent: {
      getDailyScores: (studentName: string, academicYear: string) =>
        http.get("/api/Parent/GetDailyScores", { params: { studentName, academicYear } }),
      getSubjectsAverageScores: (studentName: string, academicYear: string) =>
        http.get("/api/Parent/GetSubjectsAverageScores", {
          params: { studentName, academicYear },
        }),
      getAverageScores: (studentName: string, academicYear: string) =>
        http.get("/api/Parent/GetAverageScores", { params: { studentName, academicYear } }),
      getTuitionFeeNotification: (semesterType: SemesterType, academicYear: string) =>
        http.get("/api/Parent/GetTuitionFeeNotification", {
          params: { semesterType, academicYear },
        }),
      createPayment: (body: PaymentRequestPayload) =>
        http.post("/api/Parent/CreatePayment", body),
      paymentCallback: (params: Record<string, string | number | boolean>) =>
        http.get("/api/Parent/PaymentCallback", { params }),
    },

    orders: {
      getOrders: () => http.get("/api/Orders/GetOrders"),
      getOrderById: (orderId: string) =>
        http.get("/api/Orders/GetOrderById", { params: { orderId } }),
    },
  };
};

const shouldSkipRefresh = (url?: string) => {
  if (!url) return false;
  return (
    url.includes("/api/Authenticate/Login") ||
    url.includes("/api/Authenticate/Register") ||
    url.includes("/api/Authenticate/refresh-token") ||
    url.includes("/api/Authenticate/ConfirmEmail")
  );
};

const attachAuthInterceptors = (http: AxiosInstance, tokenStorage: TokenStorage) => {
  const refreshClient = axios.create({
    baseURL: http.defaults.baseURL,
    timeout: http.defaults.timeout,
  });

  http.interceptors.request.use((config) => {
    const accessToken = tokenStorage.getAccessToken();
    if (accessToken) {
      config.headers = config.headers ?? {};
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  });

  http.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      const original = error.config as RetryableConfig | undefined;
      const status = error.response?.status;

      if (!original || status !== 401 || original._retry || shouldSkipRefresh(original.url)) {
        return Promise.reject(error);
      }

      const refreshToken = tokenStorage.getRefreshToken();
      if (!refreshToken) {
        tokenStorage.clear();
        return Promise.reject(error);
      }

      original._retry = true;

      try {
        const refreshRes = await refreshClient.post<{ accessToken: string }>(
          "/api/Authenticate/refresh-token",
          { token: refreshToken }
        );

        tokenStorage.setTokens({ accessToken: refreshRes.data.accessToken });
        original.headers = original.headers ?? {};
        original.headers.Authorization = `Bearer ${refreshRes.data.accessToken}`;
        return http.request(original);
      } catch (refreshError) {
        tokenStorage.clear();
        return Promise.reject(refreshError);
      }
    }
  );
};

export type ManagementSchoolApi = ReturnType<typeof createApiClient>;
