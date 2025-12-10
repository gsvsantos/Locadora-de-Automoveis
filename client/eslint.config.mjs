// @ts-check
import eslint from '@eslint/js';
import { defineConfig } from 'eslint/config';
import tseslint from 'typescript-eslint';

export default defineConfig(
  // Bases recomendadas
  eslint.configs.recommended,
  tseslint.configs.recommended,
  tseslint.configs.recommendedTypeChecked, // usa info de tipos
  // Opções necessárias p/ regras "type-checked"
  {
    languageOptions: {
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  {
    rules: {
      // EXIGE tipo de retorno em TODAS as funções/métodos
      '@typescript-eslint/explicit-function-return-type': [
        'error',
        {
          allowExpressions: false,
          allowTypedFunctionExpressions: true,
          allowHigherOrderFunctions: false,
          // opcional: ignore nomes específicos (ex.: callbacks do array)
          // allowedNames: ['map', 'filter', 'reduce']
        },
      ],
      '@typescript-eslint/no-unused-vars': [
        'error',
        {
          argsIgnorePattern: '^_',
          varsIgnorePattern: '^_',
          caughtErrorsIgnorePattern: '^_',
        },
      ],

      // EXIGE tipo de parâmetros/retorno nas bordas do módulo (APIs exportadas)
      '@typescript-eslint/explicit-module-boundary-types': 'error',

      // EXIGE public/private/protected explícitos em membros de classe
      '@typescript-eslint/explicit-member-accessibility': ['error', { accessibility: 'explicit' }],

      // NÃO reclame quando você quiser tipar explicitamente valores óbvios
      // (por padrão, essa regra desencoraja "const x: number = 1")
      '@typescript-eslint/no-inferrable-types': 'off',

      // Proíbe “escapes” com any explícito
      '@typescript-eslint/no-explicit-any': ['error', { fixToUnknown: true }],

      // Opcional: evitar nomes muito curtos em variáveis (sem abreviar)
      // Ajuste exceções p/ índices de loops, etc.
      'id-length': ['warn', { min: 3, exceptions: ['i', 'j', 'k', 'x', 'y', 'id'] }],
    },
  },
);
