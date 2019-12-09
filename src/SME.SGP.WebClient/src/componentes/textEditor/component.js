import 'react-quill/dist/quill.snow.css';
import PropTypes from 'prop-types';
import React, { useEffect } from 'react';
import ReactQuill from 'react-quill';

const TextEditor = React.forwardRef((props, ref) => {
  const {
    onBlur,
    value,
    disabled,
    onClick,
    alt,
    estadoAdicional,
    maxlength,
    id,
    name,
  } = props;

  const toolbarOptions = [
    ['bold', 'italic', 'underline'],
    [{ list: 'bullet' }, { list: 'ordered' }],
  ];

  const modules = {
    toolbar: toolbarOptions,
    keyboard: {
      bindings: {
        tab: false,
      },
    },
  };

  useEffect(() => {
    if (estadoAdicional && estadoAdicional.focado) ref.current.focus();

    if (estadoAdicional && estadoAdicional.ultimoFoco)
      ref.current.setEditorSelection(
        ref.current.getEditor(),
        estadoAdicional.ultimoFoco
      );

    return () => {
      if (onBlur) {
        if (value !== ref.current.state.value) onBlur(ref.current.state.value);
      }
    };
  }, []);

  const onBlurQuill = (posicaoAnterior, origem) => {
    if (onBlur && origem === 'user') onBlur(ref.current.state.value);
  };

  const onClickQuill = posicao => {
    if (onClick) onClick(posicao);
  };

  return (
    <ReactQuill
      ref={ref}
      modules={modules}
      onBlur={onBlurQuill}
      alt={alt}
      defaultValue={value && value}
      onFocus={onClickQuill}
      readOnly={disabled}
      disabled={disabled}
      maxlength={maxlength}
      id={id}
      name={name}
    />
  );
});

TextEditor.propTypes = {
  onBlur: PropTypes.oneOfType([PropTypes.func]),
  value: PropTypes.oneOfType([PropTypes.string]),
  disabled: PropTypes.oneOfType([PropTypes.bool]),
  onClick: PropTypes.oneOfType([PropTypes.func]),
  alt: PropTypes.oneOfType([PropTypes.string]),
  estadoAdicional: PropTypes.oneOfType([PropTypes.object]),
  maxlength: PropTypes.oneOfType([PropTypes.number]),
  id: PropTypes.oneOfType([PropTypes.string]),
  name: PropTypes.oneOfType([PropTypes.string]),
};

TextEditor.defaultProps = {
  onBlur: () => {},
  value: '',
  disabled: false,
  onClick: () => {},
  alt: '',
  estadoAdicional: {},
  maxlength: 500,
  id: '',
  name: '',
};

export default TextEditor;
