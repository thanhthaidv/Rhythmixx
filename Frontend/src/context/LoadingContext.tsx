import { createContext, useContext, useState } from 'react';

interface LoadingState {
  [key: string]: boolean;
}

interface ErrorState {
  [key: string]: string | null;
}

interface LoadingContextType {
  loading: LoadingState;
  errors: ErrorState;
  setLoading: (key: string, value: boolean) => void;
  setError: (key: string, error: string | null) => void;
  getLoading: (key: string) => boolean;
  getError: (key: string) => string | null;
  clearError: (key: string) => void;
  clearAllErrors: () => void;
}

const LoadingContext = createContext<LoadingContextType | undefined>(undefined);

export const LoadingProvider = ({ children }: { children: React.ReactNode }) => {
  const [loading, setLoadingState] = useState<LoadingState>({});
  const [errors, setErrorsState] = useState<ErrorState>({});

  const setLoading = (key: string, value: boolean) => {
    setLoadingState(prev => ({ ...prev, [key]: value }));
  };

  const setError = (key: string, error: string | null) => {
    setErrorsState(prev => ({ ...prev, [key]: error }));
  };

  const getLoading = (key: string) => loading[key] ?? false;

  const getError = (key: string) => errors[key] ?? null;

  const clearError = (key: string) => {
    setErrorsState(prev => {
      const newErrors = { ...prev };
      delete newErrors[key];
      return newErrors;
    });
  };

  const clearAllErrors = () => {
    setErrorsState({});
  };

  return (
    <LoadingContext.Provider
      value={{
        loading,
        errors,
        setLoading,
        setError,
        getLoading,
        getError,
        clearError,
        clearAllErrors,
      }}
    >
      {children}
    </LoadingContext.Provider>
  );
};

export const useLoading = () => {
  const context = useContext(LoadingContext);
  if (!context) {
    throw new Error('useLoading must be used within LoadingProvider');
  }
  return context;
};

/**
 * Hook for managing loading and error state for a specific operation
 * Usage:
 * const { loading, error, setLoading, setError } = useLoadingState('upload');
 */
export const useLoadingState = (key: string) => {
  const context = useLoading();

  return {
    loading: context.getLoading(key),
    error: context.getError(key),
    setLoading: (value: boolean) => context.setLoading(key, value),
    setError: (error: string | null) => context.setError(key, error),
    clearError: () => context.clearError(key),
    isLoading: context.getLoading(key),
    hasError: !!context.getError(key),
  };
};
