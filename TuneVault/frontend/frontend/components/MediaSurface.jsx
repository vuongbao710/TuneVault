
function MediaSurface({ isVideo }) {
  const containerRef = React.useRef(null);
  React.useEffect(() => {
    const el = mediaPlayer.getElement();
    el.style.width = '100%';
    el.style.height = '100%';
    el.style.objectFit = 'cover';
    if (containerRef.current && el.parentElement !== containerRef.current) {
      containerRef.current.appendChild(el);
    }
  }, []);
  return (
    <div
      ref={containerRef}
      className={isVideo ? 'aspect-video w-28 flex-shrink-0 overflow-hidden rounded bg-black' : 'h-0 w-0 overflow-hidden'}
    />
  );
}
